using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWC.IMS.Common.HelperClasses;
using System.Diagnostics;

namespace TWC.IMS.Common.DL
{
    public class SmsOtpResponses : IDisposable
    {
        public async Task<IEnumerable<Common.Models.SmsOtpResponse>> GetListAsync()
        {
            try
            {
                using (var db = new Common.Models.CommonEntities())
                {
                    var query = from p in db.SmsOtpResponses.AsNoTracking().AsQueryable()
                                select p;

                    return await query.ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Common.Models.SmsOtpResponse> GetAsync(int id)
        {
            try
            {
                using (var db = new Common.Models.CommonEntities())
                {
                    var obj = await db.SmsOtpResponses.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Common.Models.SmsOtpResponse> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Common.Models.CommonEntities())
                {
                    var obj = await db.SmsOtpResponses.AsNoTracking()
                                                      .AsQueryable()
                                                      .FirstOrDefaultAsync(a => a.UniqueKey == uniqueKey)
                                                      .ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> InsertAsync(Common.Models.SmsOtpResponse obj)
        {
            try
            {
                using (var db = new Common.Models.CommonEntities())
                {
                    db.SmsOtpResponses.Add(obj);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                    return obj.Id;
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
        // ~SmsOtpResponses() {
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
