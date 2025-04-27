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
    public class EmailLogs : IDisposable
    {
        private string _username;

        public EmailLogs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.EmailLog>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.EmailLogs.AsNoTracking().AsQueryable()
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

        public async Task<IEnumerable<Models.EmailLog>> GetListAsync(DateTime date)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.EmailLogs.AsNoTracking().AsQueryable()
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

        public async Task<IEnumerable<Models.EmailLog>> GetListAsync(DateTime sentDateStart, DateTime sentDateEnd)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.EmailLogs.AsNoTracking().AsQueryable()
                                where p.Created != null &&
                                      p.Created.Value.Date >= sentDateStart &&
                                      p.Created.Value.Date <= sentDateEnd
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

        public async Task<IEnumerable<Models.EmailLog>> GetListAsync(params string[] recipientEmail)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.EmailLogs.AsNoTracking().AsQueryable()
                                where recipientEmail.Contains(p.To)
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

        public async Task<IEnumerable<Models.EmailLog>> GetListByResentDateAsync(DateTime resentDate)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.EmailLogs.AsNoTracking().AsQueryable()
                                where p.Created != null &&
                                      p.Created.Value.Day == resentDate.Day &&
                                      p.Created.Value.Month == resentDate.Month &&
                                      p.Created.Value.Year == resentDate.Year
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
        
        public async Task<IEnumerable<Models.ChartModels.PageHitsModel>> GetRecipientHitCountListAsync(int month, int year, int top = 5)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.EmailLogs.AsNoTracking()
                                                      .AsQueryable()
                                                      .Where(a => a.Created != null &&
                                                                  a.Created.Value.Month == month &&
                                                                  a.Created.Value.Year == year)
                                select p;

                    return await query.GroupBy(a => new
                    {
                        a.To,
                        a.Created.Value.Month,
                        a.Created.Value.Year,
                        a.CreatedBy
                    })
                    .Select(a => new Models.ChartModels.PageHitsModel
                    {
                        Activity = a.Key.To,
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

        public async Task<IEnumerable<Models.EmailLog>> GetListByResentDateAsync(DateTime resentDateStart, DateTime resentDateEnd)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.EmailLogs.AsNoTracking().AsQueryable()
                                where p.Created != null &&
                                      p.Created.Value.Date >= resentDateStart.Date &&
                                      p.Created.Value.Date <= resentDateEnd.Date
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

        public async Task<Models.EmailLog> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.EmailLogs.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.EmailLog> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.EmailLogs.AsNoTracking()
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
        // ~ErrorLogs() {
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
