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
    public class Inventory_Entries : IDisposable
    {
      
        private DL.Inventory_Entries _dlObj = null;
        private string _username;


        public Inventory_Entries(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

   
        public Task<IEnumerable<Models.Inventory_Entry>> GetListAsync(params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Inventory_Entries(username))
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

        public Task<Models.Inventory_Entry> GetByCodeAsync(Guid UniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Inventory_Entries(username))
                {
                    return _dlObj.GetByCodeAsync(UniqueKey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.Inventory_Entry> GetAsync(int businessUnitId, params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Inventory_Entries(username))
                {
                    return _dlObj.GetAsync(businessUnitId, includeEntities);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> InsertAsync(Models.Inventory_Entry obj)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Inventory_Entries(username))
                {
                    return _dlObj.InsertAsync(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public string GenerateSecretKey(int length)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Inventory_Entries(username))
                {
                    return _dlObj.GenerateSecretKey(length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateAsync(Models.Inventory_Entry obj)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Inventory_Entries(username))
                {
                    return _dlObj.UpdateAsync(obj);
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
                using (_dlObj = new DL.Inventory_Entries(username))
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
