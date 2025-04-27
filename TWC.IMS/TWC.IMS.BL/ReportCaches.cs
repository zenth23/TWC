using TWC.IMS.Models.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class ReportCaches : IDisposable
    {
        private TWC.IMS.Common.Logger _logger = null;
        private DL.ReportCaches _dlObj = null;
        private BL.SystemConfigs _scBL = null;
        private string _username;

        public ReportCaches(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<Models.ReportCache> GetAsync(Guid reportId)
        {
            string username = _username;
            try
            {
                _dlObj = new DL.ReportCaches(username);
                return await _dlObj.GetAsync(reportId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.ReportCache> GetLastAsync()
        {
            string username = _username;
            try
            {
                _dlObj = new DL.ReportCaches(username);
                return await _dlObj.GetLastAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.ReportCache> GetLastAsync(string fileName)
        {
            string username = _username;
            try
            {
                _dlObj = new DL.ReportCaches(username);
                return await _dlObj.GetLastAsync(fileName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<DateTime> DeleteInsertAsync(string fileName, string contentType, string base64)
        {
            string username = _username;
            try
            {
                _scBL = new BL.SystemConfigs(username);
                var configValue = await _scBL.GetValueAsync(SystemConfigName.REPORTS_EXPIRATION_DURATION_IN_DAYS).ConfigureAwait(false);

                DateTime today = DateTime.Now;
                var obj = new Models.ReportCache
                {
                    ContentType = contentType,
                    ExpirationDate = DateTime.Now.AddDays(Convert.ToDouble(configValue)),
                    ReportImage = base64,
                    ReportName = fileName.Trim().ToLower(),
                    UniqueKey = Guid.NewGuid(),
                    Created = today,
                    CreatedBy = username
                };

                _dlObj = new DL.ReportCaches(username);
                var checkFileName = await _dlObj.GetLastAsync(fileName).ConfigureAwait(false);

                if (checkFileName != null && checkFileName.ReportName == fileName.ToLower())
                {
                    await _dlObj.DeleteAsync(fileName).ConfigureAwait(false);
                    await _dlObj.InsertAsync(obj).ConfigureAwait(false);
                    return today;
                }
                else
                {
                    await _dlObj.InsertAsync(obj).ConfigureAwait(false);
                    return today;
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
                    if (_logger != null)
                    {
                        _logger.Dispose();
                        _logger = null;
                    }

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
        // ~ReportCaches() {
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
