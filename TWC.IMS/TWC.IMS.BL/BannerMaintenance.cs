using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TWC.IMS.DL;
using TWC.IMS.Models;

namespace TWC.IMS.BL
{
    public class BannerMaintenanceBL : IDisposable
    {
        private readonly string _username;
        private BannerMaintenanceDL _dataLayer;

        public BannerMaintenanceBL(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
            _dataLayer = new BannerMaintenanceDL(_username);
        }

        public async Task<IEnumerable<BannerMaintenance>> GetListAsync(params string[] includes)
        {
            try
            {
                return await _dataLayer.GetListAsync(includes).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<BannerMaintenance> GetAsync(int id, params string[] includes)
        {
            try
            {
                return await _dataLayer.GetAsync(id, includes).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task AddAsync(BannerMaintenance item)
        {
            try
            {
                await _dataLayer.AddAsync(item).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(BannerMaintenance item)
        {
            try
            {
                await _dataLayer.UpdateAsync(item).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _dataLayer.DeleteAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dataLayer?.Dispose();
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
