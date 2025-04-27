// REQUEST FOR REMOVAL

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
    public class Requests : IDisposable
    {
        private string _username;

        public Requests(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<List<Models.Request>> GetListAsync(params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {

                    var query = (DbQuery<Models.Request>)db.Set<Models.Request>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .OrderBy(x => x.Name)
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


        public async Task<Models.Request> GetAsync(int id, params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {

                    var query = (DbQuery<Models.Request>)db.Set<Models.Request>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.Id == id)
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.Request> GetAsync(Guid uniqueKey, params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {

                    var query = (DbQuery<Models.Request>)db.Set<Models.Request>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.UniqueKey == uniqueKey)
                                      .ConfigureAwait(false);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> InsertAsync(Models.Request obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.Requests.Add(obj);
                    await db.SaveChangesAsync(_username).ConfigureAwait(false);

                    return obj.Id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateAsync(Models.Request obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.Entry(obj).State = EntityState.Modified;
                    return await db.SaveChangesAsync(username).ConfigureAwait(false);
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception(TWC.IMS.Common.Messages.RECORD_MODIFIED);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entity = await ex.Entries.Single().GetDatabaseValuesAsync().ConfigureAwait(false);
                if (entity == null)
                {
                    throw new Exception(TWC.IMS.Common.Messages.RECORD_DELETED);
                }
                else
                {
                    throw new Exception(TWC.IMS.Common.Messages.RECORD_MODIFIED);
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
        // ~SystemConfigs() {
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
