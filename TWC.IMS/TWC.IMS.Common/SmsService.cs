using Newtonsoft.Json;
using TWC.IMS.Common.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public class SmsService
    {
        private static SmsService _instance;
        private static readonly object _lockObj = new object();

        public string ApplicationVersion { get; set; }
        public string UserRole { get; set; }
        public string Environment { get; set; }
        public bool IsMobileDevice { get; set; }
        public string ClientIPAddress { get; set; }
        public string UserAgent { get; set; }

        private SmsService() { }

        public static SmsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        _instance = new SmsService();
                    }
                }
                return _instance;
            }
        }

        public Task LogSmsOtpResponseAsync(string username, string smsServiceResponse, string sid = null)
        {
            var responseObj = JsonConvert.DeserializeObject<MittoSmsServiceResponseModel>(smsServiceResponse);
            string sid2 = sid ?? responseObj.Sid;
            string status = responseObj.Status;
            DateTime? validityDate = responseObj.ValidUntil;
            string description = responseObj.Description;

            using (var dl = new DL.SmsOtpResponses())
            {
                var obj = new Common.Models.SmsOtpResponse
                {
                    Created = DateTime.Now,
                    UniqueKey = Guid.NewGuid(),
                    CreatedBy = username,
                    Sid = sid2,
                    Status = status,
                    ValidUntil = validityDate,
                    Description = description
                };

                return dl.InsertAsync(obj);
            }
        }

        public async Task<string> GenerateSmsOtpCodeAsync(string username, string phoneNumber, string customMessage)
        {
            try
            {
                using (var scDL = new DL.SystemConfigs())
                {
                    string recipientMobileNo = phoneNumber;
                    string apiUsername = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_USERNAME).ConfigureAwait(false);
                    string password = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_PASSWORD).ConfigureAwait(false);
                    string originator = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_ORIGINATOR).ConfigureAwait(false);
                    string apiUrl = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_URL).ConfigureAwait(false);
                    string mittoSmsBaseApiUrl = string.Format(apiUrl, apiUsername, password, originator, recipientMobileNo, customMessage);

                    using (var client = new HttpClient())
                    {
                        // get response
                        string response = await client.GetStringAsync(mittoSmsBaseApiUrl).ConfigureAwait(false);
                        // log response to DB                
                        var _ = LogSmsOtpResponseAsync(username, response);

                        var obj = JsonConvert.DeserializeObject<MittoSmsServiceResponseModel>(response);
                        return obj.Sid;
                    }
                }
            }
            catch (Exception ex)
            {
                // mask phone number
                string firstSeven = phoneNumber.Substring(0, 7);
                phoneNumber = phoneNumber.Replace(firstSeven, "*");
                string msg = $"Error generating OTP via SMS for Phone #{phoneNumber}";
                var _ = Logger.ErrorAsync(msg, username, ApplicationVersion, UserRole, Environment, ClientIPAddress, UserAgent, ex, isMobileDevice: IsMobileDevice);
            }
            return null;
        }

        public async Task<bool> ValidateSmsOtpCodeAsync(string username, string phoneNumber, string sid, string code)
        {
            try
            {
                using (var scDL = new DL.SystemConfigs())
                {
                    string recipientMobileNo = phoneNumber;
                    string apiUsername = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_USERNAME).ConfigureAwait(false);
                    string password = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_PASSWORD).ConfigureAwait(false);
                    string originator = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_ORIGINATOR).ConfigureAwait(false);
                    string apiUrl = await scDL.GetValueAsync(HelperClasses.SystemConfigName.SMS_API_URL_VALIDATE).ConfigureAwait(false);
                    string mittoSmsBaseApiUrl = string.Format(apiUrl, apiUsername, password, originator, recipientMobileNo, sid, code);

                    using (var client = new HttpClient())
                    {
                        // get response
                        string response = await client.GetStringAsync(mittoSmsBaseApiUrl).ConfigureAwait(false);
                        // log response to DB                
                        var _ = LogSmsOtpResponseAsync(username, response, sid);

                        var obj = JsonConvert.DeserializeObject<MittoSmsServiceResponseModel>(response);
                        return string.Compare(obj.Status, "ACK", true) == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // mask phone number
                string firstSeven = phoneNumber.Substring(0, 7);
                phoneNumber = phoneNumber.Replace(firstSeven, "*");
                string msg = $"Error generating OTP via SMS for Phone #{phoneNumber}";
                var _ = Logger.ErrorAsync(msg, username, ApplicationVersion, UserRole, Environment, ClientIPAddress, UserAgent, ex, isMobileDevice: IsMobileDevice);
            }
            return false;
        }
    }
}
