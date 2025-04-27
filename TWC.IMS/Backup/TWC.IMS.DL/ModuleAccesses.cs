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
    public class ModuleAccesses : IDisposable
    {
        private string _username;

        public ModuleAccesses(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.ModuleAccess>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.ModuleAccesses.Include(a => a.Access)
                                                           .Include(a => a.Module)
                                                           .Include(a => a.RolePermissions)
                                                           .AsNoTracking()
                                                           .AsQueryable()
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

        public async Task<IEnumerable<Models.ModuleAccess>> GetListAsync(int moduleId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.ModuleAccesses.Include(a => a.Module)
                                                           .Include(a => a.Access)
                                                           .Include(a => a.RolePermissions)
                                                           .AsNoTracking()
                                                           .AsQueryable()
                                where p.ModuleAccess_Module == moduleId
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

        public async Task<Models.ModuleAccess> GetAsync(int moduelId, int accessId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.ModuleAccesses.AsNoTracking()
                                                     .AsQueryable()
                                                     .Include(a => a.Module)
                                                     .Include(a => a.Access)
                                                     .Where(a => a.ModuleAccess_Module == moduelId &&
                                                                 a.ModuleAccess_Access == accessId)
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

        public async Task<Models.ModuleAccess> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.ModuleAccesses.AsNoTracking()
                                                     .AsQueryable()
                                                     .Include(a => a.Module)
                                                     .Include(a => a.Access)
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

        public async Task<Models.ModuleAccess> GetAsync(int moduelId, string accessName)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.ModuleAccesses.AsNoTracking()
                                                     .AsQueryable()
                                                     .Include(a => a.Module)
                                                     .Include(a => a.Access)
                                                     .Where(a => a.ModuleAccess_Module == moduelId &&
                                                                 string.Compare(a.Access.Name.Trim(), accessName.Trim(), true) == 0)
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

        /// <summary>
        /// Returns the object if already in use
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="accessId"></param>
        /// <returns></returns>
        public async Task<Models.ModuleAccess> ModuleAccessInUseAsync(int moduleId, int accessId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var ma = await db.ModuleAccesses.Include(a => a.Access)
                                                    .AsNoTracking()
                                                    .AsQueryable()
                                                    .Where(a => a.ModuleAccess_Module == moduleId && 
                                                                a.ModuleAccess_Access == accessId)
                                                    .FirstOrDefaultAsync()
                                                    .ConfigureAwait(false);
                    if (ma != null)
                    {
                        var isInUse = await db.RolePermissions.AnyAsync(a => a.RolePermission_ModuleAccess == ma.Id).ConfigureAwait(false);
                        if (isInUse)
                            return ma;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> InsertAsync(Models.ModuleAccess obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.ModuleAccesses.Add(obj);
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

        public async Task<int> UpdateAsync(Models.ModuleAccess obj)
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
                    var obj = await db.ModuleAccesses.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.ModuleAccesses.Remove(obj);
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
                    var obj = await db.ModuleAccesses.FirstOrDefaultAsync(a => a.UniqueKey == uniqueKey).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.ModuleAccesses.Remove(obj);
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

        public async Task<int> DeleteByModuleAsync(int moduleId)
        {
            string username = _username;
            using (var db = new Models.Entities())
            {
                var module = await db.Modules.Include(a => a.ModuleAccesses).FirstAsync(a => a.Id == moduleId).ConfigureAwait(false);
                try
                {
                    db.ModuleAccesses.RemoveRange(module.ModuleAccesses.ToList());
                    return await db.SaveChangesAsync(username).ConfigureAwait(false);
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

        public async Task<int> DeleteByModuleAsync(int moduleId, int accessId)
        {
            string username = _username;
            using (var db = new Models.Entities())
            {
                var accesses = await db.ModuleAccesses.Where(a => a.ModuleAccess_Module == moduleId && 
                                                                  a.ModuleAccess_Access == accessId)
                                                      .ToListAsync()
                                                      .ConfigureAwait(false);
                try
                {
                    db.ModuleAccesses.RemoveRange(accesses);
                    return await db.SaveChangesAsync(username).ConfigureAwait(false);
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
        // ~ModuleAccesses() {
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
