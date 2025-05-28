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
    public class CatalogMaintenanceBL : IDisposable
    {
        private DL.CatalogMaintenanceDL _dlObj = null;
        private string _username;

        public CatalogMaintenanceBL(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        // Get a list of CatalogMaintenance items with optional includes
        public Task<IEnumerable<CatalogMaintenance>> GetListAsync(params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.CatalogMaintenanceDL(username))
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

        // Get a single CatalogMaintenance item by ID with optional includes
        public Task<CatalogMaintenance> GetAsync(int id, params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.CatalogMaintenanceDL(username))
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

        // Add a new CatalogMaintenance record
        public async Task AddAsync(CatalogMaintenance catalog)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.CatalogMaintenanceDL(username))
                {
                    // Assuming `CreatedBy` is handled by the business layer and passed
                    catalog.CreatedBy = username; // Set created by
                    await _dlObj.AddAsync(catalog).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // Delete an existing CatalogMaintenance record by ID
        public async Task DeleteAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.CatalogMaintenanceDL(username))
                {
                    await _dlObj.DeleteAsync(id).ConfigureAwait(false);
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
                    // Dispose managed resources (i.e., DL object)
                    if (_dlObj != null)
                    {
                        _dlObj.Dispose();
                        _dlObj = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
