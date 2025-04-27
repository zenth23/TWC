using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TWC.IMS.DL.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWC.IMS.Models;

namespace TWC.IMS.DL
{
    public class UserActivityLogs : IDisposable
    {
        private string _username;

        public UserActivityLogs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<Models.UserActivityLog>> GetListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.UserActivityLogs.AsNoTracking().AsQueryable()
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

        public async Task<IEnumerable<Models.UserActivityLog>> GetListAsync(DataSourceRequest request)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.UserActivityLogs.AsNoTracking().AsQueryable()
                                select p;

                    query = query.ApplyFiters(request.Filters);
                    query = query.ApplySort(request.Sorts);
                    query = query.ApplyPaging(request.Page, request.PageSize);

                    return await query.ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.UserActivityLog>> GetListByUserAsync(string username)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.UserActivityLogs.AsNoTracking()
                                                             .AsQueryable()
                                                             .Where(a => string.Compare(a.CreatedBy, username.Trim(), true) == 0)
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

        public async Task<IEnumerable<Models.ChartModels.PageHitsModel>> GetHitCountListAsync(int month, int year, int top = 5)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.UserActivityLogs.AsNoTracking()
                                                             .AsQueryable()
                                                             .Where(a => a.Created != null &&
                                                                         a.Created.Value.Month == month &&
                                                                         a.Created.Value.Year == year)
                                select p;

                    return await query.GroupBy(a => new
                    {
                        a.Activity,
                        a.Created.Value.Month,
                        a.Created.Value.Year,
                        a.CreatedBy
                    })
                    .Select(a => new Models.ChartModels.PageHitsModel
                    {
                        Activity = a.Key.Activity,
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

        public async Task<DataSourceResult> GetListAsync(DateTime date, DataSourceRequest request)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.UserActivityLogs.AsNoTracking().AsQueryable()
                                where p.Created != null &&
                                      p.Created.Value.Month == date.Month &&
                                      p.Created.Value.Year == date.Year
                                select p;

                    var result = await query.ToDataSourceResultAsync(request).ConfigureAwait(false);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.UserActivityLog>> GetListAsync(DateTime date)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.UserActivityLogs.AsNoTracking().AsQueryable()
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

        public async Task<Models.UserActivityLog> GetAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.UserActivityLogs.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.UserActivityLog> GetAsync(Guid uniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.UserActivityLogs.AsNoTracking()
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

        public async Task<int> InsertAsync(Models.UserActivityLog obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.UserActivityLogs.Add(obj);
                    await db.SaveChangesAsync().ConfigureAwait(false); // do not audit log
                    return obj.Id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.UserActivityLog>> GetListAsync(string activity, string username, DateTime? startDate, DateTime endDate)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    return await db.UserActivityLogs
                             .AsNoTracking()
                             .Where(x =>
                                string.Compare(x.Activity, activity, true) == 0
                                && string.Compare(x.CreatedBy, username) == 0)
                             .Select(x => new
                             {
                                 Record = x,
                                 Created = x.Created.HasValue
                                ? DbFunctions.CreateDateTime(x.Created.Value.Year, x.Created.Value.Month, x.Created.Value.Day, (int?)x.Created.Value.Hour, x.Created.Value.Minute, x.Created.Value.Second)
                                : null
                             })
                             .Where(x =>
                                (startDate.HasValue ? x.Created >= startDate.Value : true)
                                && x.Created <= endDate
                             )
                             .Select(x => x.Record)
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

        public async Task<IEnumerable<Models.UserActivityLog>> GetListRecentActivitiesAsync(string username, int? numOfRows = 20)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.UserActivityLogs.AsNoTracking().AsQueryable()
                                                             .OrderByDescending(c => c.Created)
                                                             .Where(a => string.Compare(a.CreatedBy, username.Trim(), true) == 0)
                                select p;
                    query = query.Take(numOfRows ?? 20);
                    return await query.ToListAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
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
        // ~UserActivityLogs() {
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
