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
    public class SupportTool : IDisposable
    {
        private DL.SupportTool _dlObj = null;
        private string _username;

        public SupportTool(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.SQLColumn>> GetColumnListAsync(int id = 0)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SupportTool())
                {
                    return _dlObj.GetColumnListAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.SQLTable>> GetTableListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SupportTool())
                {
                    return _dlObj.GetTableListAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.SQLTable> GetTableAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SupportTool())
                {
                    return _dlObj.GetTableAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<object> GetRecordsAsync(Type type, string sqlQuery, params object[] parameters)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SupportTool())
                {
                    return _dlObj.GetRecordsAsync(type, sqlQuery, parameters);
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
        // ~SupportTool() {
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
