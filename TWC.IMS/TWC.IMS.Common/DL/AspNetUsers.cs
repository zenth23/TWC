using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.Common.DL
{
    public class AspNetUsers : IDisposable
    {
        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var query = db.AspNetUsers.AsNoTracking()
                                              .AsQueryable()
                                              .Where(p => p.UserName == username)
                                              .Select(a => a.UserName); // do not use Distinct

                    var count = await query.CountAsync().ConfigureAwait(false);
                    return count == 0 ? true : false;
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UsernameDL() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
