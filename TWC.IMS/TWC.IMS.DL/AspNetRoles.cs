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
    public class AspNetRoles : IDisposable
    {
        private string _username;

        public AspNetRoles(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.AspNetRole>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.AspNetRoles.Include(a => a.RoleDetails)
                                                        .Include(a => a.AspNetUsers)
                                                        .Include(a => a.RolePermissions)
                                                        .AsNoTracking().AsQueryable()
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

        public async Task<Models.AspNetRole> GetAsync(string id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.AspNetRoles.Include(a => a.RoleDetails)
                                                  .Include(a => a.AspNetUsers)
                                                  .Include(a => a.RolePermissions)
                                                  .FirstOrDefaultAsync(a => string.Compare(a.Id, id.Trim(), true) == 0)
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

        public async Task<Models.AspNetRole> GetByNameAsync(string name)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.AspNetRoles.Include(a => a.RoleDetails)
                                                  .Include(a => a.AspNetUsers)
                                                  .Include(a => a.RolePermissions)
                                                  .FirstOrDefaultAsync(a => string.Compare(a.Name, name.Trim(), true) == 0)
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

        public async Task<string> InsertAsync(Models.AspNetRole obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.AspNetRoles.Add(obj);
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

        public async Task<int> UpdateAsync(Models.AspNetRole obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var existing = await db.AspNetRoles.FindAsync(obj.Id).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.Name = obj.Name.Trim();                        
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    throw new NullReferenceException(TWC.IMS.Common.Messages.RECORD_NOT_FOUND);
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
                    var obj = await db.AspNetRoles.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.AspNetRoles.Remove(obj);
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
        // ~AspNetRoles() {
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
