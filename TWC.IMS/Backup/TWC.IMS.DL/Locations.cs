using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Models.HelperModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.DL
{
    public class Locations : IDisposable
    {
        private string _username;


        public Locations(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }



        public async Task<IEnumerable<Models.Location>> GetListAsync(params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Location>)db.Set<Models.Location>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .OrderBy(x => x.location_name)
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        
        public async Task<Models.Location> GetAsync(int id ,params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Location>)db.Set<Models.Location>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()

                                      .FirstOrDefaultAsync(a=> a.Id == id)
                                      .ConfigureAwait(false);
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
                _username = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Inventory() {
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
