using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.DL
{
    public class UserDetails : IDisposable
    {
        private string _username;

        public UserDetails(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<bool> IsAccountActiveAsync(string username)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    DateTime today = DateTime.Now;
#if DEBUG
                    var inspector = db.UserDetails.AsNoTracking()
                                                  .AsQueryable()
                                                  .Include(a => a.AspNetUser)
                                                  .Where(a => a.AspNetUser.UserName.Trim() == username.Trim())
                                                  .Select(a => new
                                                  {
                                                      a.AspNetUser.UserName,
                                                      a.AspNetUser.LockoutEndDateUtc,
                                                      a.ExpirationDatetime,
                                                      a.DeactivationDatetime,
                                                      a.ActivationDatetime
                                                  });
#endif

                    var result = await db.UserDetails.AsNoTracking()
                                                     .AsQueryable()
                                                     .Include(a => a.AspNetUser)
                                                     .AnyAsync(a => a.AspNetUser.UserName.Trim() == username.Trim() &&
                                                                    (a.AspNetUser.LockoutEndDateUtc == null || a.AspNetUser.LockoutEndDateUtc <= today) &&
                                                                    (a.ExpirationDatetime == null || a.ExpirationDatetime >= today) &&
                                                                    (a.DeactivationDatetime == null || a.DeactivationDatetime >= today) &&
                                                                    (a.ActivationDatetime == null || a.ActivationDatetime <= today))
                                                     .ConfigureAwait(false);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.UserDetail>> GetListAsync(bool? isActive = null, params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {

                    var query = (DbQuery<Models.UserDetail>)db.Set<Models.UserDetail>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });


                    if (isActive.HasValue)
                        return await query.Where(a => a.IsActive == isActive.Value).ToListAsync().ConfigureAwait(false);

                    return await query.ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.UserDetail> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.UserDetails.Include(a => a.AspNetUser)
                                                  .AsNoTracking()
                                                  .AsQueryable()
                                                  .FirstOrDefaultAsync(a => a.Id == id)
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

        public async Task<Models.UserDetail> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.UserDetails.Include(a => a.AspNetUser)
                                                  .AsNoTracking()
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

        public async Task<Models.UserDetail> GetByUsernameAsync(string username)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.UserDetails.Include(a => a.AspNetUser)
                                                  .AsNoTracking()
                                                  .AsQueryable()
                                                  .Where(a => string.Compare(a.AspNetUser.UserName.Trim(), username.Trim(), true) == 0)
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

        public async Task<Models.UserDetail> GetByUserIdAsync(string userId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.UserDetails.Include(a => a.AspNetUser)
                                                  .AsNoTracking()
                                                  .AsQueryable()
                                                  .Where(a => string.Compare(a.AspNetUser.Id.Trim(), userId.Trim(), true) == 0)
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

        public async Task<int> InsertAsync(Models.UserDetail obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.UserDetails.Add(obj);
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

        public async Task<int> ActivateUserAsync(int id)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var existing = await db.UserDetails.FindAsync(id).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.ActivationDatetime = DateTime.Now;
                        existing.Modified = DateTime.Now;
                        existing.ModifiedBy = username;
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

        public async Task<int> DeactivateUserAsync(int id)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var existing = await db.UserDetails.FindAsync(id).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.DeactivationDatetime = DateTime.Now;
                        existing.Modified = DateTime.Now;
                        existing.ModifiedBy = username;
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

        public async Task<int> UpdateStatusAsync(string userId, string status)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var existing = await db.UserDetails.FirstOrDefaultAsync(a => a.UserDetail_AspNetUser == userId).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.Status = status;
                        existing.Modified = DateTime.Now;
                        existing.ModifiedBy = username;
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

        public async Task<int> SetUserLastLoginDatetimeAsync(int id)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var existing = await db.UserDetails.FindAsync(id).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.LastLoginDatetime = DateTime.Now;
                        existing.Modified = DateTime.Now;
                        existing.ModifiedBy = username;
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    return 0;
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

        public async Task<int> UpdateAsync(Models.UserDetail obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var existing = await db.UserDetails.FindAsync(obj.Id).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.Avatar = obj.Avatar;
                        existing.AvatarMimeType = obj.AvatarMimeType;
                        existing.EmployeeId = obj.EmployeeId;
                        existing.FirstName = obj.FirstName;
                        existing.LastName = obj.LastName;
                        existing.MiddleName = obj.MiddleName;
                        existing.Suffix = obj.Suffix;
                        existing.Nickname = obj.Nickname;
                        existing.Modified = obj.Modified;
                        existing.ModifiedBy = obj.ModifiedBy;
                        existing.IsActive = obj.IsActive;
                        existing.ActivationDatetime = obj.ActivationDatetime;
                        //existing.LastLoginDatetime = obj.LastLoginDatetime;   <-- this must not be here
                        existing.DeactivationDatetime = obj.DeactivationDatetime;
                        existing.ExpirationDatetime = obj.ExpirationDatetime;
                        existing.UserDetail_AspNetUser = obj.UserDetail_AspNetUser;
                        existing.Status = obj.Status;
                        existing.RowVersion = obj.RowVersion;

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
                    var obj = await db.UserDetails.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.UserDetails.Remove(obj);
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
                    var obj = await db.UserDetails.FirstOrDefaultAsync(a => a.UniqueKey == uniqueKey).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.UserDetails.Remove(obj);
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
        // ~UserDetails() {
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
