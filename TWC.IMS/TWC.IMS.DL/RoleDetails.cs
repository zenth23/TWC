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
    public class RoleDetails : IDisposable
    {
        private string _username;

        public RoleDetails(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.RoleDetail>> GetListAsync(bool? isActive = null)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.RoleDetails.Include(a => a.AspNetRole).AsNoTracking().AsQueryable()
                                select p;

                    if (isActive.HasValue)
                        query = query.Where(a => a.IsActive == isActive.Value);

                    return await query.ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.RoleDetail>> GetListAsync(IEnumerable<string> roles)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = db.RoleDetails.AsNoTracking()
                                              .Include(a => a.AspNetRole)
                                              .Where(a => roles.Contains(a.AspNetRole.Name));

                    return await query.ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.RoleDetail> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.RoleDetails.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<Models.RoleDetail>> GetRoleWithUsersAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.RoleDetails.Include("AspNetRole.AspNetUsers.UserDetails")
                                                  .AsNoTracking()
                                                  .AsQueryable()
                                                  .Where(a => a.Id == id)
                                                  .ToListAsync()
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

        public async Task<Models.RoleDetail> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.RoleDetails.AsNoTracking()
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

        public async Task<Models.RoleDetail> GetAsync(string roleId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.RoleDetails.AsNoTracking()
                                                  .Where(a => string.Compare(a.RoleDetail_AspNetRole, roleId.Trim(), true) == 0)
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

        public async Task<bool> HasActiveRoleAsync(IEnumerable<string> roleIds)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var flag = await db.RoleDetails.AsNoTracking()
                                                   .AsQueryable()
                                                   .Where(a => roleIds.Contains(a.RoleDetail_AspNetRole) &&
                                                               a.IsActive)
                                                   .AnyAsync()
                                                   .ConfigureAwait(false);
                    return flag;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.RoleDetail> GetByNameAsync(string roleName)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.RoleDetails.Include(a => a.AspNetRole)
                                                  .AsNoTracking()
                                                  .AsQueryable()
                                                  .Where(a => string.Compare(a.AspNetRole.Name, roleName.Trim(), true) == 0)
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

        public async Task<int> InsertAsync(Models.RoleDetail obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.RoleDetails.Add(obj);
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

        public async Task<int> UpdateAsync(Models.RoleDetail obj)
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
                    var obj = await db.RoleDetails.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.RoleDetails.Remove(obj);
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

        public async Task<int> DeleteAsync(Guid uniqueKey)
        {
            string username = _username;
            using (var db = new Models.Entities())
            {
                try
                {
                    var obj = await db.RoleDetails.FirstOrDefaultAsync(a => a.UniqueKey == uniqueKey).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.RoleDetails.Remove(obj);
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
        // ~RoleDetails() {
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
