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
    public class PasswordHistories : IDisposable
    {
        private DL.PasswordHistories _dlObj = null;
        private string _username;

        public PasswordHistories(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.PasswordHistory>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.PasswordHistories(username))
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

        public Task<Models.PasswordHistory> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.PasswordHistories(username))
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

        public Task<Models.PasswordHistory> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.PasswordHistories(username))
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

        public Task<Models.PasswordHistory> GetCurrentPasswordAsync(string userId)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.PasswordHistories(username))
                {
                    return _dlObj.GetCurrentPasswordAsync(userId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.PasswordHistory>> GetHistoricalPasswordsByUserIdAsync(string userId, int numOfRows)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.PasswordHistories(username))
                {
                    return _dlObj.GetHistoricalPasswordsByUserIdAsync(userId, numOfRows);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> InsertAsync(Models.PasswordHistory obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.UniqueKey = Guid.NewGuid();
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    using (_dlObj = new DL.PasswordHistories(username))
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
        // ~PasswordHistories() {
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
