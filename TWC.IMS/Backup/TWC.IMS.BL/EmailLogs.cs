using TWC.IMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class EmailLogs : IDisposable
    {
        private DL.EmailLogs _dlObj = null;
        private string _username;

        public EmailLogs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.EmailLog>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
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

        public Task<IEnumerable<Models.EmailLog>> GetListAsync(DateTime date)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
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

        public Task<IEnumerable<Models.EmailLog>> GetListAsync(params string[] recipientEmail)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
                {
                    return _dlObj.GetListAsync(recipientEmail);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.EmailLog>> GetListAsync(DateTime sentDateStart, DateTime sentDateEnd)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
                {
                    return _dlObj.GetListAsync(sentDateStart, sentDateEnd);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.EmailLog>> GetListByResentDateAsync(DateTime resentDate)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
                {
                    return _dlObj.GetListByResentDateAsync(resentDate);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.EmailLog>> GetListByResentDateAsync(DateTime resentDateStart, DateTime resentDateEnd)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
                {
                    return _dlObj.GetListByResentDateAsync(resentDateStart, resentDateEnd);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.ChartModels.PageHitsModel>> GetRecipientHitCountListAsync(DateTime date)
        {
            try
            {
                using (_dlObj = new DL.EmailLogs(_username))
                {
                    return _dlObj.GetRecipientHitCountListAsync(date.Month, date.Year, 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.ChartModels.AdminDashboardCounterModel> GetLogCountersAsync(DateTime date, IEnumerable<Models.EmailLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
                {
                    if (list == null)
                    {
                        list = await _dlObj.GetListAsync(date).ConfigureAwait(false);
                        list = list.Where(a => a.Created.HasValue);
                    }

                    int totalErrors = list.Count();
                    int totalLoggedUsers = list.Select(a => new { a.To }).Distinct().Count();

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

        public Task<Models.EmailLog> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
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

        public Task<Models.EmailLog> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.EmailLogs(username))
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
        // ~ErrorLogs() {
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
