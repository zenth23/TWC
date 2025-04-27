using TWC.IMS.Models;
using TWC.IMS.Models.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class StatusSets : IDisposable
    {
        private DL.StatusSets _dlObj = null;
        private string _username;

        public StatusSets(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.StatusSet>> GetListAsync()
        {
            string username = _username;
            try
            {
                _dlObj = new DL.StatusSets(username);
                return _dlObj.GetListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.StatusSet>> GetUserAccountStatusListAsync()
        {
            string username = _username;
            try
            {
                _dlObj = new DL.StatusSets(username);
                return _dlObj.GetUserAccountStatusListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.StatusSet> GetAsync(int id)
        {
            string username = _username;
            try
            {
                _dlObj = new DL.StatusSets(username);
                return _dlObj.GetAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.StatusSet> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                _dlObj = new DL.StatusSets(username);
                return _dlObj.GetAsync(uniqueKey);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.StatusSet> GetAsync(string name)
        {
            string username = _username;
            try
            {
                _dlObj = new DL.StatusSets(username);
                return _dlObj.GetAsync(name);
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
        // ~StatusSets() {
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
