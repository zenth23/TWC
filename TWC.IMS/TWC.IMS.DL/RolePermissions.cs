using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.DL
{
    public class RolePermissions : IDisposable
    {
        private string _username;

        public RolePermissions(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.RolePermission>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.RolePermissions.Include(a => a.AspNetRole)
                                                            .Include(a => a.ModuleAccess.Module)
                                                            .Include(a => a.ModuleAccess.Access)
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

        public async Task<IEnumerable<Models.RolePermission>> GetListForUserAccessMatrixAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = db.RolePermissions.AsNoTracking()
                                                  .AsQueryable()
                                                  .Include(a => a.AspNetRole)
                                                  .Include(a => a.ModuleAccess.Module)
                                                  .Include(a => a.ModuleAccess.Access);

                    return await query.ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.HelperModels.AccessControlListReportModel>> GetListForAccessControlListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var permissions = (await db.RolePermissions.AsNoTracking()
                                                              .AsQueryable()
                                                              .Include(a => a.ModuleAccess.Module)
                                                              .Include(a => a.ModuleAccess.Access)
                                                              .ToListAsync()
                                                              .ConfigureAwait(false))
                                                              .Select(a => new
                                                              {
                                                                  RoleId = a.RolePermission_Role,
                                                                  AccessName = $"{a.ModuleAccess.Module.Name}.{a.ModuleAccess.Access.Name}"
                                                              });

                    var details = await db.Database.SqlQuery<Models.HelperModels.AccessControlListReportModel>("spGetAccessControlList").ToListAsync().ConfigureAwait(false);

                    var list = details.Select((d, i) => new Models.HelperModels.AccessControlListReportModel
                    {
                        No = i + 1,
                        ActivationDatetime = d.ActivationDatetime,
                        DaysInactive = d.DaysInactive,
                        DeactivationDatetime = d.DeactivationDatetime,
                        ExpirationDate = d.ExpirationDate,
                        IsActive = d.IsActive,
                        LastLoginDatetime = d.LastLoginDatetime,
                        Username = d.Username,
                        UserRole = d.UserRole,
                        EmployeeId = d.EmployeeId,
                        FirstName = d.FirstName,
                        MiddleName = d.MiddleName,
                        LastName = d.LastName,
                        Modules = string.Join(", ", permissions.Where(a => string.Compare(a.RoleId, d.RoleId, true) == 0)
                                                               .OrderBy(a => a.AccessName)
                                                               .Select(a => a.AccessName)),
                    });

                    return list;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.RolePermission>> GetListAsync(string roleId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.RolePermissions.AsNoTracking().AsQueryable()
                                where string.Compare(p.RolePermission_Role.Trim(), roleId.Trim(), true) == 0
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

        public async Task<IEnumerable<Models.RolePermission>> GetListByRoleNameAsync(string roleName)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.RolePermissions.Include(a => a.AspNetRole)
                                                            .Include(a => a.ModuleAccess.Module)
                                                            .Include(a => a.ModuleAccess.Access)
                                                            .AsNoTracking()
                                                            .AsQueryable()
                                where string.Compare(p.AspNetRole.Name.Trim(), roleName.Trim(), true) == 0
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

        public async Task<Models.RolePermission> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.RolePermissions.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.RolePermission> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.RolePermissions.AsNoTracking()
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

        public async Task<int> InsertAsync(Models.RolePermission obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.RolePermissions.Add(obj);
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

        public async Task<int> UpdateAsync(Models.RolePermission obj)
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
                    var obj = await db.RolePermissions.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.RolePermissions.Remove(obj);
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
                    var obj = await db.RolePermissions.FirstOrDefaultAsync(a => a.UniqueKey == uniqueKey).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.RolePermissions.Remove(obj);
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

        public async Task<int> DeleteByRoleAsync(string roleId)
        {
            string username = _username;
            using (var db = new Models.Entities())
            {
                try
                {
                    var itemsToDelete = await db.RolePermissions.Where(a => string.Compare(a.RolePermission_Role, roleId, true) == 0)
                                                                .ToListAsync()
                                                                .ConfigureAwait(false);
                    db.RolePermissions.RemoveRange(itemsToDelete);
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
        // ~RolePermissions() {
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
