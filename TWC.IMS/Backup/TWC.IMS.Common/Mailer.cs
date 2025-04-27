using TWC.IMS.Common.DL;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Common.HelperModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public class Mailer
    {
        private static Mailer _instance;
        private static readonly object _lockObj = new object();

        public string ApplicationVersion { get; set; }
        public string UserRole { get; set; }
        public string Environment { get; set; }
        public string ClientIPAddress { get; set; }
        public string UserAgent { get; set; }
        public bool IsMobileDevice { get; set; }

        private Mailer() { }

        public static Mailer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        _instance = new Mailer();
                    }
                }
                return _instance;
            }
        }

        public async Task SendMailAsync(string eventName, string username, string subject, string body, string[] to, string[] cc = null, string[] bcc = null)
        {
            #region Email Address Validation Section

            var invalidEmailAddress = new List<string>();
            if (to == null)
                throw new ArgumentNullException("Recipient ('to') argument must not be null.");
            else if (to.Length < 1)
                throw new ArgumentOutOfRangeException("Recipient ('to') argument must have at least one value.");
            else
            {
                // filter out empty, null, whitespace values
                to = to.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                // validate email address
                foreach (var email in to)
                {
                    var isValid = await Tools.IsValidEmailAddress(email).ConfigureAwait(false);
                    if (!isValid)
                        invalidEmailAddress.Add(email);
                }
            }

            if (cc == null)
                cc = new string[] { };
            else
            {
                // filter out empty, null, whitespace values
                cc = cc.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                // validate email address
                foreach (var email in cc)
                {
                    var isValid = await Tools.IsValidEmailAddress(email).ConfigureAwait(false);
                    if (!isValid)
                        invalidEmailAddress.Add(email);
                }
            }

            if (bcc == null)
                bcc = new string[] { };
            else
            {
                // filter out empty, null, whitespace values
                bcc = bcc.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                // validate email address
                foreach (var email in bcc)
                {
                    var isValid = await Tools.IsValidEmailAddress(email).ConfigureAwait(false);
                    if (!isValid)
                        invalidEmailAddress.Add(email);
                }
            }

            if (invalidEmailAddress.Count > 0)
                throw new InvalidDataException($"Invalid email addresses: {string.Join(",", invalidEmailAddress)}");

            #endregion

            var m = MethodBase.GetCurrentMethod();
            string mName = m == null ? "-" : m.ReflectedType == null ? "--" : m.ReflectedType.FullName;

            try
            {
                using (var scBL = new SystemConfigs())
                {
                    string host = await scBL.GetValueAsync(SystemConfigName.SMTP_HOST).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(host))
                        throw new NullReferenceException("SMTP_HOST is undefined.");

                    string tmpPort = await scBL.GetValueAsync(SystemConfigName.SMTP_PORT).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(tmpPort))
                        throw new NullReferenceException("SMTP_PORT is undefined.");

                    int port = Convert.ToInt32(tmpPort);
                    string from = await scBL.GetValueAsync(SystemConfigName.EMAIL_FROM).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(from))
                        throw new NullReferenceException("SMTP EMAIL_FROM is undefined.");

                    string decryptedEmailPass = await scBL.GetValueAsync(SystemConfigName.SMTP_EMAILPASS).ConfigureAwait(false);

                    string enableSslConfig = await scBL.GetValueAsync(SystemConfigName.SMTP_ENABLESSL).ConfigureAwait(false);
                    bool enableSsl = false;
                    bool.TryParse(enableSslConfig, out enableSsl);

                    string smtpMailerName = await scBL.GetValueAsync(SystemConfigName.SMTP_MAILER_NAME).ConfigureAwait(false);
                    smtpMailerName = smtpMailerName ?? "NOREPLY Mailer";

                    using (var msg = new MailMessage())
                    {
                        msg.BodyEncoding = Encoding.UTF8;
                        msg.IsBodyHtml = true;
                        msg.Body = body;
                        msg.Subject = subject;
                        msg.From = new MailAddress(from, smtpMailerName.Trim());

                        foreach (var email in to)
                        {
                            msg.To.Add(email);
                        }
                        foreach (var email in cc)
                        {
                            msg.CC.Add(email);
                        }
                        foreach (var email in bcc)
                        {
                            msg.Bcc.Add(email);
                        }

                        using (SmtpClient client = new SmtpClient())
                        {
                            client.Host = host;
                            client.Port = port;
                            client.EnableSsl = enableSsl;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;

                            if (host.ToLower().Contains("gmail"))
                            {
                                client.UseDefaultCredentials = false;
                                client.Credentials = new NetworkCredential(from, decryptedEmailPass);
                            }
                            else
                                client.UseDefaultCredentials = true;

                            // then send
                            await client.SendMailAsync(msg).ConfigureAwait(false);

                            try
                            {
                                // then log email but don't wait
                                var _ = Logger.LogEmailAsync(from, string.Join(",", to), string.Join(",", cc), string.Join(",", bcc), subject, body, username);
                            }
                            catch (Exception ex)
                            {
                                string errMsg = $"{eventName.Trim().ToUpper()}:::Error logging email log";
                                var _ = Logger.ErrorAsync(errMsg, username, ApplicationVersion, UserRole, Environment, ClientIPAddress, UserAgent, ex, isMobileDevice: IsMobileDevice);
                                // do not throw email logging error
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg = $"{eventName.Trim().ToUpper()}:::Error sending mail";
                var _ = Logger.ErrorAsync(errMsg, username, ApplicationVersion, UserRole, Environment, ClientIPAddress, UserAgent, ex, isMobileDevice: IsMobileDevice);
            }
        }

    }
}
