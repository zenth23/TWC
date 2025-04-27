using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWC.IMS.Models.ChartModels;
using System.Diagnostics;

namespace TWC.IMS.DL
{
    public class AuditLogs : IDisposable
    {
        private string _username;

        public AuditLogs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.AuditLog>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.AuditLogs.AsNoTracking().AsQueryable()
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

        public async Task<IEnumerable<Models.AuditLog>> GetListAsync(DateTime date)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.AuditLogs.AsNoTracking().AsQueryable()
                                where p.Created != null &&
                                      p.Created.Value.Month == date.Month &&
                                      p.Created.Value.Year == date.Year
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

        public async Task<IEnumerable<Models.ChartModels.PageHitsModel>> GetTableHitCountListAsync(int month, int year, int top = 5)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.AuditLogs.AsNoTracking()
                                                      .AsQueryable()
                                                      .Where(a => a.Created != null &&
                                                                  a.Created.Value.Month == month &&
                                                                  a.Created.Value.Year == year)
                                select p;

                    return await query.GroupBy(a => new
                    {
                        a.TableName,
                        a.Created.Value.Month,
                        a.Created.Value.Year,
                        a.CreatedBy
                    })
                    .Select(a => new Models.ChartModels.PageHitsModel
                    {
                        Activity = a.Key.TableName,
                        HitCount = a.Count(),
                        Month = a.Key.Month,
                        Username = a.Key.CreatedBy,
                        Year = a.Key.Year
                    })
                    .OrderByDescending(a => a.HitCount)
                    .Take(top)
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

        public async Task<Models.AuditLog> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.AuditLogs.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.AuditLog> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.AuditLogs.AsNoTracking()
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

        public async Task<int> InsertAsync(Models.AuditLog obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.AuditLogs.Add(obj);
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
        // ~AuditLogs() {
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
