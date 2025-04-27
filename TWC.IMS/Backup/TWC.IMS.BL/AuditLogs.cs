using TWC.IMS;
using TWC.IMS.Models;
using TWC.IMS.Models.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class AuditLogs : IDisposable
    {
        private DL.AuditLogs _dlObj = null;
        private string _username;

        public AuditLogs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.AuditLog>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AuditLogs(username))
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

        public Task<IEnumerable<Models.AuditLog>> GetListAsync(DateTime date)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AuditLogs(username))
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

        public Task<IEnumerable<Models.ChartModels.PageHitsModel>> GetTableHitCountListAsync(DateTime date)
        {
            try
            {
                using (_dlObj = new DL.AuditLogs(_username))
                {
                    return _dlObj.GetTableHitCountListAsync(date.Month, date.Year, 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.ChartModels.AdminDashboardCounterModel> GetLogCountersAsync(DateTime date, IEnumerable<Models.AuditLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AuditLogs(username))
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

        public Task<Models.AuditLog> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AuditLogs(username))
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

        public Task<Models.AuditLog> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AuditLogs(username))
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

        public Task<int> InsertAsync(Models.AuditLog obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.UniqueKey = Guid.NewGuid();
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    using (_dlObj = new DL.AuditLogs(username))
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

        public Task<int> CreateAspNetUsersLockoutEndDateUtcModifiedEventAsync(string oldValue, string newValue, string userId)
        {
            string username = _username;
            try
            {
                var obj = new AuditLog();
                obj.ColumnName = "LockoutEndDateUtc";
                obj.EventType = AuditLogEventType.MODIFIED.ToString();
                obj.NewValue = newValue;
                obj.OldValue = oldValue;
                obj.RowID = userId;
                obj.TableName = "AspNetUsers";
                obj.UniqueKey = Guid.NewGuid();
                obj.Created = DateTime.Now;
                obj.CreatedBy = username;

                using (_dlObj = new DL.AuditLogs(username))
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
        // ~AuditLogs() {
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
