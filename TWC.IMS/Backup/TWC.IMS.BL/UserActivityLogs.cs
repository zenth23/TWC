using Kendo.Mvc.UI;
using TWC.IMS;
using TWC.IMS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.BL
{
    public class UserActivityLogs : IDisposable
    {
        private DL.UserActivityLogs _dlObj = null;
        private string _username;

        public UserActivityLogs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.UserActivityLog>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    return _dlObj.GetListAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.UserActivityLog>> GetListAsync(DataSourceRequest request)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    return _dlObj.GetListAsync(request);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.UserActivityLog>> GetListAsync(DateTime date)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    return _dlObj.GetListAsync(date);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<DataSourceResult> GetListAsync(DateTime date, DataSourceRequest request)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    return _dlObj.GetListAsync(date, request);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.UserActivityLog>> GetListByUserAsync(string username)
        {
            string username2 = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username2))
                {
                    return _dlObj.GetListByUserAsync(username);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.UserActivityLog> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    return _dlObj.GetAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.UserActivityLog> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    return _dlObj.GetAsync(uniqueKey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> InsertAsync(Models.UserActivityLog obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.UniqueKey = Guid.NewGuid();
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    using (_dlObj = new DL.UserActivityLogs(username))
                    {
                        return _dlObj.InsertAsync(obj);
                    }
                }
                else throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodType">GET / POST</param>
        /// <param name="absoluteUrl">URL</param>
        /// <param name="activity">Activity to log</param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(string methodType, string absoluteUrl, string activity, string ipAddress, string userAgent,
                                     string appVersion, bool isMobileDevice, string sessionId, DateTime? sessionStart,
                                     int sessionTimeout, string userRole, string formData)
        {
            string username = _username;
            try
            {
                var obj = new UserActivityLog();
                obj.AbsoluteUrl = absoluteUrl.Trim();
                obj.Activity = activity.Trim();
                obj.MethodType = methodType.Trim();
                obj.ClientIPAddress = ipAddress;
                obj.UserAgent = userAgent;
                obj.UniqueKey = Guid.NewGuid();
                obj.Created = DateTime.Now;
                obj.CreatedBy = username;
                obj.AppVersion = appVersion;
                obj.IsMobileDevice = isMobileDevice;
                obj.SessionId = sessionId;
                obj.SessionStart = sessionStart;
                obj.SessionTimeout = sessionTimeout;
                obj.UserRole = userRole;
                obj.FormData = formData;

                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    return _dlObj.InsertAsync(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.ChartModels.AdminDashboardCounterModel> GetLogCountersAsync(DateTime date, IEnumerable<Models.UserActivityLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserActivityLogs(username))
                {
                    if (list == null)
                    {
                        list = await _dlObj.GetListAsync(date).ConfigureAwait(false);
                        list = list.Where(a => a.Created.HasValue);
                    }

                    int totalErrors = list.Count();
                    int totalLoggedUsers = list.Select(a => new { a.CreatedBy }).Distinct().Count();

                    return new Models.ChartModels.AdminDashboardCounterModel
                    {
                        TotalErrors = totalErrors,
                        TotalLoggedUsers = totalLoggedUsers
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.UserActivityLog>> GetListAsync(string activity, string username, DateTime? startDate, DateTime endDate)
        {
            try
            {
                using (_dlObj = new DL.UserActivityLogs(_username))
                {
                    return _dlObj.GetListAsync(activity, username, startDate, endDate);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.UserActivityLog>> GetListRecentActivitiesAsync(string username)
        {
            string username2 = _username;
            try
            {
                _dlObj = new DL.UserActivityLogs(username);
                return  _dlObj.GetListRecentActivitiesAsync(username);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.ChartModels.PageHitsModel>> GetHitCountListAsync(DateTime date)
        {
            try
            {
                using (_dlObj = new DL.UserActivityLogs(_username))
                {
                    return _dlObj.GetHitCountListAsync(date.Month, date.Year, 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
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
                    if (_dlObj != null)
                    {
                        _dlObj.Dispose();
                        _dlObj = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserActivityLogs() {
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
