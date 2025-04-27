using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Models.HelperModels;
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
    public class Product_Inventory : IDisposable
    {
        private string _username;


        public Product_Inventory(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }


        public string GenerateSecretKey(int length)
        {
            try
            {
                string acceptedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
                Random rand = new Random();
                string result = string.Empty;

                for (int i = 0; i < length; i++)
                {
                    int temp = rand.Next(0, acceptedChars.Length);
                    result += acceptedChars.ToCharArray()[temp].ToString();
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

     
        public async Task<IEnumerable<Models.Product_Inventory>> GetListAsync(params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Product_Inventory>)db.Set<Models.Product_Inventory>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
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

        public async Task<Models.Product_Inventory> GetByCodeAsync(Guid UniqueKey)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Product_Inventory>)db.Set<Models.Product_Inventory>();

                    return await query.Include(a=> a.Product_Master).AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.Product_Master.UniqueKey == UniqueKey)
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.Product_Inventory> GetAsync(int supplierId, params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Product_Inventory>)db.Set<Models.Product_Inventory>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.supplier_id == supplierId)
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        
        public async Task<Models.Product_Inventory> GetAsync(int product_id, int location_id, int supplier_id, params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Product_Inventory>)db.Set<Models.Product_Inventory>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.supplier_id == supplier_id
                                                          && x.location_id == location_id
                                                          && x.product_id == product_id)
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> InsertAsync(Models.Product_Inventory obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.Product_Inventory.Add(obj);
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

        public async Task<int> UpdateAsync(Models.Product_Inventory obj)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    db.Entry(obj).State = EntityState.Modified;
                    await db.SaveChangesAsync(username).ConfigureAwait(false);
                    return obj.Id;
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
            try
            {
                using (var db = new Models.Entities())
                {
                    var dbObj = await db.Product_Inventory.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                    if (dbObj != null)
                    {
                        db.Product_Inventory.Remove(dbObj);
                        await db.SaveChangesAsync().ConfigureAwait(false);

                        return 1;
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
                _username = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Inventory() {
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
