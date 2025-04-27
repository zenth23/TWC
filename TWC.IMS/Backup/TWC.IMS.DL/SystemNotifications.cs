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
    public class SystemNotifications: IDisposable
    {
        private string _username;

        public SystemNotifications(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.SystemNotification>> GetListAsync(int userId, string[] includeEntities)
        {
            try
            {                
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.SystemNotification>)db.Set<Models.SystemNotification>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .Where(x => x.SystemNotification_UserDetail == userId)
                                      .OrderByDescending(x => x.Created)
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

        public async Task<Models.SystemNotification> GetAsync(int id, string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.SystemNotification>)db.Set<Models.SystemNotification>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task<List<Models.SystemNotification>> GetByUserAsync(int id, string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.SystemNotification>)db.Set<Models.SystemNotification>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                .Where(x => x.SystemNotification_UserDetail == id)
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
        public async Task<int> InsertAsync(Models.SystemNotification obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.SystemNotifications.Add(obj);
                    // no audit log
                    await db.SaveChangesAsync().ConfigureAwait(false);
                    return obj.Id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateAsync(Models.SystemNotification obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var dbObj = await db.SystemNotifications.FindAsync(obj.Id).ConfigureAwait(false);
                    if (dbObj != null)
                    {
                        dbObj.IsViewed = obj.IsViewed;
                        dbObj.SeenDate = obj.SeenDate;
                        // no audit log
                        await db.SaveChangesAsync().ConfigureAwait(false);

                        return obj.Id;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<Models.SystemNotification>> GetAsync(string title, string caption, string description, string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.SystemNotification>)db.Set<Models.SystemNotification>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                .Where(x => string.Compare(x.Title, title, true) == 0
                                        &&  string.Compare(x.Caption, caption, true) == 0
                                        && string.Compare(x.Description, description, true) == 0)
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
        // ~ApprovalTypes() {
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
