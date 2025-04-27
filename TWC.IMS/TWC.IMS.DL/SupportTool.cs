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
    public class SupportTool : IDisposable
    {
        public async Task<IEnumerable<Models.SQLColumn>> GetColumnListAsync(int id = 0)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    if (id == 0)
                    {
                        var query = from p in db.SQLColumn.AsNoTracking().AsQueryable()
                                    select p;
                        return await query.ToListAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        var query = from p in db.SQLColumn.Where(x => x.TableId == id).AsNoTracking().AsQueryable()
                                    select p;
                        return await query.ToListAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<Models.SQLTable>> GetTableListAsync()
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = from p in db.SQLTable.AsNoTracking().AsQueryable().OrderBy(x => x.TableName)
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

        public async Task<Models.SQLTable> GetTableAsync(int id)
        {
            try
            {
                using (var db = new Models.Entities())
                {

                    var obj = await db.SQLTable.FindAsync(id).ConfigureAwait(false);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<object> GetRecordsAsync(Type type, string sqlQuery, params object[] parameters)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.Database.SqlQuery(type, sqlQuery, parameters).ToListAsync().ConfigureAwait(false);
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

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SupportTool() {
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
