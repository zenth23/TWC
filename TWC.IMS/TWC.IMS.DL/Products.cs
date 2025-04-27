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
using System.Data.Entity.Validation;

namespace TWC.IMS.DL
{
    public class Products : IDisposable
    {
        private string _username;


        public Products(string username)
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


        public async Task<IEnumerable<Models.Product_Master>> GetListAsync(params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Product_Master>)db.Set<Models.Product_Master>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .OrderBy(x => x.product_name)
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

        public async Task<Models.Product_Master> GetByNameAsync(string name)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Product_Master>)db.Set<Models.Product_Master>();

                    return await query.AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.product_name.ToLower() == name.ToLower())
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.Product_Master> GetAsync(int productId, params string[] includeEntities)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    var query = (DbQuery<Models.Product_Master>)db.Set<Models.Product_Master>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()

                                      .FirstOrDefaultAsync(a => a.Id == productId)
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> InsertAsync(Models.Product_Master obj)
        {
            try
            {
                using (var db = new Models.Entities())
                {
                    db.Product_Master.Add(obj);
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

        public async Task<int> UpdateAsync(Models.Product_Master model)
        {
            string username = _username;
            try
            {
                using (var db = new Models.Entities())
                {
                    var obj = await db.Product_Master.FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (obj != null)
                    {

                        obj.gemstones = model.gemstones;
                        obj.karat = model.karat;
                        obj.material = model.material;
                        obj.product_name = model.product_name;
                        obj.ProductType_id = model.ProductType_id;
                        obj.retail_price = model.retail_price;
                        obj.selling_price = model.selling_price;
                        //obj.weight = model.weight;
                        obj.LowStockThreshold = model.LowStockThreshold;
                        obj.Modified = DateTime.Now;
                        obj.ModifiedBy = username;

                        //db.Entry(obj).State = EntityState.Modified;
                        await db.SaveChangesAsync(username).ConfigureAwait(false);
                    }
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
                    var dbObj = await db.Product_Master.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                    if (dbObj != null)
                    {
                        db.Product_Master.Remove(dbObj);
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
