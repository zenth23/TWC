using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.Common.DL
{
    public class SystemConfigs : IDisposable
    {
        public async Task<IEnumerable<Models.SystemConfig>> GetListAsync()
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var query = from p in db.SystemConfigs.AsNoTracking().AsQueryable()
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

        public async Task<Models.SystemConfig> GetAsync(int id, bool autoDecrypt = true)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.SystemConfigs.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        string name = obj.Name;
                        string value = obj.Value;
                        if (autoDecrypt)
                            obj.Value = await SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name, value).ConfigureAwait(false);
                    }
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.SystemConfig> GetAsync(Guid uniqueKey, bool autoDecrypt = true)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.SystemConfigs.AsNoTracking().AsQueryable()
                                                    .FirstOrDefaultAsync(a => Guid.Equals(a.UniqueKey, uniqueKey))
                                                    .ConfigureAwait(false);
                    if (obj != null)
                    {
                        string name = obj.Name;
                        string value = obj.Value;
                        if (autoDecrypt)
                            obj.Value = await SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name, value).ConfigureAwait(false);
                    }
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Models.SystemConfig> GetAsync(SystemConfigName name, bool autoDecrypt = true)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.SystemConfigs.AsNoTracking().Where(a => string.Compare(a.Name.Trim(), name.ToString().Trim(), true) == 0)
                                                                   .FirstOrDefaultAsync()
                                                                   .ConfigureAwait(false);
                    if (obj != null)
                    {
                        string value = obj.Value;
                        if (autoDecrypt)
                            obj.Value = await SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name.ToString(), value).ConfigureAwait(false);
                    }
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<string> GetValueAsync(int id, bool autoDecrypt = true)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.SystemConfigs.FindAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        string name = obj.Name;
                        string value = obj.Value;
                        if (autoDecrypt)
                            value = await SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name, value).ConfigureAwait(false);
                        return value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<string> GetValueAsync(Guid uniqueKey, bool autoDecrypt = true)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.SystemConfigs.AsNoTracking().AsQueryable()
                                                    .FirstOrDefaultAsync(a => Guid.Equals(a.UniqueKey, uniqueKey))
                                                    .ConfigureAwait(false);
                    if (obj != null)
                    {
                        string name = obj.Name;
                        string value = obj.Value;
                        if (autoDecrypt)
                            value = await SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name, value).ConfigureAwait(false);
                        return value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<string> GetValueAsync(SystemConfigName name, bool autoDecrypt = true)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.SystemConfigs.AsNoTracking().Where(a => string.Compare(a.Name.Trim(), name.ToString().Trim(), true) == 0)
                                                                   .FirstOrDefaultAsync()
                                                                   .ConfigureAwait(false);
                    if (obj != null)
                    {
                        string value = obj.Value;
                        if (autoDecrypt)
                            value = await SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name.ToString(), value).ConfigureAwait(false);
                        return value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<string> GetEncryptValueConfigsValueAsync()
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.SystemConfigs.AsNoTracking().Where(a => string.Compare(a.Name.Trim(), SystemConfigName.ENCRYPT_VALUE_CONFIGS.ToString(), true) == 0)
                                                                   .FirstOrDefaultAsync()
                                                                   .ConfigureAwait(false);
                    if (obj != null)
                    {
                        return obj.Value;
                    }
                    return null;
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
        // ~SystemConfigsDL() {
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
