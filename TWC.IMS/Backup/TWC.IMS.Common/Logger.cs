using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Common.Models;
using TWC.IMS.Common.DL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

namespace TWC.IMS.Common
{
    public class Logger : IDisposable
    {
        private static DL.Logger _lDL = null;

        public static async Task WarningAsync(string message, string username, string appVersion, string userRole, string environment, string clientIPAddress, string userAgent, [CallerMemberName]string methodName = "_", bool isMobileDevice = false, string paramData = null)
        {
            if (string.IsNullOrWhiteSpace(message) ||
                string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException();

            var obj = new ErrorLog();
            obj.UniqueKey = Guid.NewGuid();
            obj.Created = DateTime.Now;
            obj.CreatedBy = username.Trim();
            obj.ErrorMessage = message;
            obj.ErrorNumber = 0;
            obj.FriendlyErrorMessage = message;
            obj.MessageType = MessageType.WARNING.ToString();
            obj.MethodName = methodName;
            obj.Exception = null;
            obj.AppVersion = appVersion;
            obj.UserRole = userRole;
            obj.IsMobileDevice = isMobileDevice;
            obj.Environment = environment;
            obj.ImpactLevel = ImpactLevel.Medium.ToString();
            obj.ClientIPAddress = clientIPAddress;
            obj.UserAgent = userAgent;
            obj.ParamData = paramData;

            _lDL = new DL.Logger();
            await _lDL.LogAsync(obj).ConfigureAwait(false);
        }

        public static async Task InformationAsync(string message, string username, string appVersion, string userRole, string environment, string clientIPAddress, string userAgent, [CallerMemberName]string methodName = "_", bool isMobileDevice = false, string paramData = null)
        {
            if (string.IsNullOrWhiteSpace(message) ||
                string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException();

            var obj = new ErrorLog();
            obj.UniqueKey = Guid.NewGuid();
            obj.Created = DateTime.Now;
            obj.CreatedBy = username.Trim();
            obj.ErrorMessage = message;
            obj.ErrorNumber = 0;
            obj.FriendlyErrorMessage = message;
            obj.MessageType = MessageType.INFORMATION.ToString();
            obj.MethodName = methodName;
            obj.Exception = null;
            obj.AppVersion = appVersion;
            obj.UserRole = userRole;
            obj.IsMobileDevice = isMobileDevice;
            obj.Environment = environment;
            obj.ImpactLevel = ImpactLevel.Low.ToString();
            obj.ClientIPAddress = clientIPAddress;
            obj.UserAgent = userAgent;
            obj.ParamData = paramData;

            _lDL = new DL.Logger();
            await _lDL.LogAsync(obj).ConfigureAwait(false);
        }

        public static async Task ErrorAsync(string friendlyErrorMessage, string username, string appVersion, string userRole, string environment, string clientIPAddress, string userAgent, Exception exception, [CallerMemberName]string methodName = "_", bool isMobileDevice = false, int? errorNumber = null, string paramData = null)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new NullReferenceException("Method name is required");
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException("Username is required");

            var obj = new ErrorLog();
            obj.UniqueKey = Guid.NewGuid();
            obj.Created = DateTime.Now;
            obj.CreatedBy = username.Trim();
            obj.FriendlyErrorMessage = friendlyErrorMessage;
            obj.MessageType = MessageType.ERROR.ToString();
            obj.MethodName = methodName.Trim();
            obj.ParamData = paramData;
            obj.AppVersion = appVersion;
            obj.UserRole = userRole;
            obj.IsMobileDevice = isMobileDevice;
            obj.Environment = environment;
            obj.ImpactLevel = ImpactLevel.High.ToString();
            obj.ClientIPAddress = clientIPAddress;
            obj.UserAgent = userAgent;

            if (exception != null)
            {
                obj.ErrorMessage = exception.InnerException == null ? exception.Message : exception.InnerException.Message;
                obj.ErrorNumber = errorNumber == null ? exception.HResult : errorNumber.Value;
                var ex = new
                {
                    exception.Data,
                    exception.InnerException,
                    exception.Message,
                    exception.Source,
                    exception.StackTrace
                };
                obj.Exception = Tools.ObjectToJson(ex);
            }
            else
                obj.ErrorMessage = friendlyErrorMessage;

            _lDL = new DL.Logger();
            await _lDL.LogAsync(obj).ConfigureAwait(false);
        }

        /// <summary>
        /// When calling this method, wrap this in Task.Run() unless called inside a try-catch or using statements
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task LogEmailAsync(string from, string to, string cc, string bcc, string subject, string body, string username)
        {
            if (string.IsNullOrWhiteSpace(from) ||
                string.IsNullOrWhiteSpace(to) ||
                string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(body) ||
                string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException();

            var obj = new EmailLog();
            obj.UniqueKey = Guid.NewGuid();
            obj.Created = DateTime.Now;
            obj.CreatedBy = username.Trim();
            obj.Bcc = bcc;
            obj.Body = body;
            obj.Cc = cc;
            obj.From = from;
            obj.Subject = subject;
            obj.To = to;

            _lDL = new DL.Logger();
            await _lDL.LogEmailAsync(obj).ConfigureAwait(false);
        }

        public static async Task UpdateEmailLogStatusAsync(int emailLogId, StatusType statusType, string username)
        {
            _lDL = new DL.Logger();
            await _lDL.UpdateEmailLogStatusAsync(emailLogId, statusType, username).ConfigureAwait(false);
        }

        public static async Task WriteLogFileAsync(string path, string appName, string message)
        {
            try
            {
                Directory.CreateDirectory(path);

                string fileName = string.Format("LOG_{0}_{1:" + TWC.IMS.Common.StringFormats.DATE_FORMAT_SHORT_9 + "}.log", appName, DateTime.Now);
                string pathName = Path.Combine(path, fileName);
                string formattedMsg = string.Format("{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_LONG_2 + "} > {1}\n", DateTime.Now, message);
                byte[] encodedText = Encoding.Unicode.GetBytes(formattedMsg);

                using (FileStream fs = new FileStream(pathName, mode: FileMode.Append, access: FileAccess.Write, share: FileShare.ReadWrite, useAsync: true, bufferSize: 4096))
                {
                    await fs.WriteAsync(encodedText, 0, encodedText.Length).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static Task LogToEventViewer(string message, int eventID, short category = 0)
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = Messages.EVENTVIEWER_APP_NAME;
                    eventLog.WriteEntry(message, EventLogEntryType.Error, eventID, category);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Task.FromResult(0);
        }

        public static Task LogToEventViewer(string message, short category = 0)
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    int eventID = Messages.EVENTVIEWER_APP_EVENT_ID;
                    eventLog.Source = Messages.EVENTVIEWER_APP_NAME;
                    eventLog.WriteEntry(message, EventLogEntryType.Error, eventID, category);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Task.FromResult(0);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (_lDL != null)
                    {
                        _lDL.Dispose();
                        _lDL = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Logger() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
