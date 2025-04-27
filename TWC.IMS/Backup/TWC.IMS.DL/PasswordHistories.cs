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
    public class PasswordHistories: IDisposable
    {
        private string _username;

        public PasswordHistories(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.PasswordHistory>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.PasswordHistories.AsNoTracking().AsQueryable()
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

        public async Task<Models.PasswordHistory> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.PasswordHistories.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.PasswordHistory> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.PasswordHistories.AsNoTracking()
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

        public async Task<Models.PasswordHistory> GetCurrentPasswordAsync(string userId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.PasswordHistories.AsNoTracking()
                                                        .Where(a => string.Compare(a.PasswordHistory_AspNetUser, userId.Trim(), true) == 0)
                                                        .OrderByDescending(a => a.Created)
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

        public async Task<IEnumerable<Models.PasswordHistory>> GetHistoricalPasswordsByUserIdAsync(string userId, int numOfRows)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    // use lazy loading
                    var obj = await db.PasswordHistories.AsNoTracking()
                                                        .AsQueryable()
                                                        .Where(x => string.Compare(x.PasswordHistory_AspNetUser, userId.Trim(), true) == 0)
                                                        .OrderByDescending(x => x.Created)
                                                        .Take(numOfRows)
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

        public async Task<int> InsertAsync(Models.PasswordHistory obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.PasswordHistories.Add(obj);
                    await db.SaveChangesAsync().ConfigureAwait(false); // do not log changes since PasswordHistory is already an audit log for passwords
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
                _username = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PasswordHistories() {
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
