using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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
    public class AspNetUsers : IDisposable
    {
        private string _username;

        public AspNetUsers(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.AspNetUser>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.AspNetUsers.Include(a => a.UserDetails)
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

        /// <summary>
        /// Gets an AspNetuser entity filtered by AspNetUser Id.
        /// </summary>
        /// <param name="id">Id value of AspNetUser.</param>
        /// <returns>Task of Models.AspNetUser.</returns>
        public async Task<Models.AspNetUser> GetAsync(string id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.AspNetUsers.Include(a => a.UserDetails)
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

        public async Task<Models.AspNetUser> GetByUsernameAsync(string username)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.AspNetUsers.Include(a => a.UserDetails)
                                                  .AsNoTracking()
                                                  .AsQueryable()
                                                  .Where(a => string.Compare(a.UserName.Trim(), username.Trim(), true) == 0)
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

        public async Task<string> InsertAsync(Models.AspNetUser obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.AspNetUsers.Add(obj);
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

        public async Task<int> UpdateAsync(Models.AspNetUser obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var existing = await db.AspNetUsers.FindAsync(obj.Id).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.AccessFailedCount = obj.AccessFailedCount;
                        existing.Email = obj.Email.Trim();
                        existing.EmailConfirmed = obj.EmailConfirmed;
                        existing.LockoutEnabled = obj.LockoutEnabled;
                        existing.LockoutEndDateUtc = obj.LockoutEndDateUtc;
                        existing.PasswordHash = obj.PasswordHash;
                        existing.PhoneNumber = obj.PhoneNumber;
                        existing.PhoneNumberConfirmed = obj.PhoneNumberConfirmed;
                        existing.SecurityStamp = obj.SecurityStamp;
                        existing.TwoFactorEnabled = obj.TwoFactorEnabled;
                        existing.UserName = obj.UserName.Trim();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">AspNetUser Id value.</param>
        /// <param name="uname">Username of currently logged in user.</param>
        /// <returns></returns>
        public async Task<int> UpdateUsernameAsync(string userId, string uname)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var user = await db.AspNetUsers.FindAsync(userId).ConfigureAwait(false);
                    if (user != null)
                    {
                        user.UserName = uname.ToLower().Trim();
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    else
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

        public async Task<int> UpdateUserEmailAsync(string userId, string email)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var user = await db.AspNetUsers.FindAsync(userId).ConfigureAwait(false);
                    if (user != null)
                    {
                        user.Email = email.ToLower().Trim();
                        user.EmailConfirmed = false; // unverify
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    else
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

        public async Task<int> UpdatePhoneNumberAsync(string userId, string newPhoneNumber)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var user = await db.AspNetUsers.FindAsync(userId).ConfigureAwait(false);
                    if (user != null)
                    {
                        user.PhoneNumber = newPhoneNumber.Trim();
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    else
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

        public async Task<int> UpdateAllUsersSecurityStampAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var users = db.AspNetUsers.AsQueryable();
                    await users.ForEachAsync(a => a.SecurityStamp = Guid.NewGuid().ToString().ToLower());
                    return await db.SaveChangesAsync().ConfigureAwait(false);
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

        public async Task<int> UpdateSecurityStampAsync(string userId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var user = await db.AspNetUsers.FindAsync(userId).ConfigureAwait(false);
                    if (user != null)
                    {
                        string newSecurityStamp = Guid.NewGuid().ToString().ToLower();
                        user.SecurityStamp = newSecurityStamp;
                        return await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
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
                    var obj = await db.AspNetUsers.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        db.AspNetUsers.Remove(obj);
                        return await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
                    throw new NullReferenceException("Object not found.");
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
        // ~AspNetUsers() {
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
