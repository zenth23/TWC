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
    public class Accesses : IDisposable
    {
        private string _username;

        public Accesses(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.Access>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.Accesses.AsNoTracking().AsQueryable()
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

        public async Task<Models.Access> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.Accesses.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.Access> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.Accesses.AsNoTracking()
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

        public async Task<Models.Access> GetAsync(string name)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.Accesses.AsNoTracking()
                                               .AsQueryable()
                                               .Where(a => string.Compare(a.Name.Trim(), name.Trim(), true) == 0)
                                               .FirstOrDefaultAsync()
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

        public async Task<int> InsertAsync(Models.Access obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.Accesses.Add(obj);
                    await db.SaveChangesAsync(username).ConfigureAwait(false);
                    return obj.Id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateAsync(Models.Access obj)
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

        public async Task<int> DeleteAsync(int id)
        {
            string username = _username;
            using (var db = new Models.Entities())
            {
                try
                {
                    var obj = await db.Accesses.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.Accesses.Remove(obj);
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    throw new NullReferenceException(TWC.IMS.Common.Messages.RECORD_NOT_FOUND);
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
                catch (DbUpdateException duex)
                {
                    if (duex.InnerException?.InnerException?
                            .Message.IndexOf(TWC.IMS.Common.Messages.SQL_DELETE_STATEMENT_ERROR_MESSAGE, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        throw new Exception(TWC.IMS.Common.Messages.RECORD_IN_USE);
                    }
                    else throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public async Task<int> DeleteAsync(Guid uniqueKey)
        {
            string username = _username;
            using (var db = new Models.Entities())
            {
                try
                {
                    var obj = await db.Accesses.FirstOrDefaultAsync(a => a.UniqueKey == uniqueKey).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.Accesses.Remove(obj);
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    throw new NullReferenceException(TWC.IMS.Common.Messages.RECORD_NOT_FOUND);
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
                catch (DbUpdateException duex)
                {
                    if (duex.InnerException?.InnerException?
                            .Message.IndexOf(TWC.IMS.Common.Messages.SQL_DELETE_STATEMENT_ERROR_MESSAGE, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        throw new Exception(TWC.IMS.Common.Messages.RECORD_IN_USE);
                    }
                    else throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
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
        // ~Accesses() {
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
