using Newtonsoft.Json;
using TWC.IMS;
using TWC.IMS.Models;
using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Models.HelperModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.BL
{
    public class SalesOrders : IDisposable
    {
      
        private DL.SalesOrders _dlObj = null;
        private string _username;


        public SalesOrders(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

   
        public Task<IEnumerable<Models.SalesOrderHeader>> GetListAsync(params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    return _dlObj.GetListAsync(includeEntities);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.SalesOrderHeader> GetByKeyAsync(Guid uniqueKey, params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    return _dlObj.GetByKeyAsync(uniqueKey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.SalesOrderHeader> GetAsync(int id, params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    return _dlObj.GetAsync(id, includeEntities);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> InsertAsync(Models.SalesOrderHeader obj)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;
                    obj.UniqueKey = Guid.NewGuid();

                    return _dlObj.InsertAsync(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }


        public Task<int> InsertDetailAsync(Models.SalesOrderDetail obj)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    return _dlObj.InsertDetailAsync(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateAsync(Models.SalesOrderHeader obj)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    obj.Modified = DateTime.Now;
                    obj.ModifiedBy = username;

                    return _dlObj.UpdateAsync(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateDetailAsync(Models.SalesOrderDetail obj)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    obj.Modified = DateTime.Now;
                    obj.ModifiedBy = username;

                    return _dlObj.UpdateDetailAsync(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SalesOrders(username))
                {
                    return _dlObj.DeleteAsync(id);
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
                    if (_dlObj != null)
                    {
                        _dlObj.Dispose();
                        _dlObj = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

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
