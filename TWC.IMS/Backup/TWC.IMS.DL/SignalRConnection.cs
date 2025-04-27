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
    public class SignalRConnection: IDisposable
    {
        private string _username;

        public SignalRConnection(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.SignalRConnection>> GetListAsync(string userId, string[] includeEntities)
        {
            try
            {                
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.SignalRConnection>)db.Set<Models.SignalRConnection>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .AsQueryable()
                                      .Where(x => x.UserId == userId)
                                      .OrderBy(x => x.Created)
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

  
        public async Task<int> InsertAsync(Models.SignalRConnection obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.SignalRConnections.Add(obj);
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

        public async Task<int> DeleteAsync(string connectionId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var conObj = await db.SignalRConnections.FirstOrDefaultAsync(x => x.ConnectionId == connectionId).ConfigureAwait(false);
                    if (conObj != null)
                    {
                        db.SignalRConnections.Remove(conObj);
                        return await db.SaveChangesAsync().ConfigureAwait(false);
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
        public async Task<int> DeleteAllAsync(string userId)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var conObjs = await db.SignalRConnections.Where(x => x.UserId == userId).ToListAsync().ConfigureAwait(false);
                    if (conObjs != null)
                    {
                        db.SignalRConnections.RemoveRange(conObjs);
                        return await db.SaveChangesAsync().ConfigureAwait(false);
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
