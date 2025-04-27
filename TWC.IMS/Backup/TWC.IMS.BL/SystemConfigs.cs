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
    public class SystemConfigs : IDisposable
    {
        #region PRIVATE MEMBERS
        private DL.SystemConfigs _dlObj = null;
        private string _username;

        /// <summary>
        /// Returns true if config was initially created
        /// </summary>
        /// <param name="configNameToAppend"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task<bool> UpdateEncryptValueConfigAsync(string configNameToAppend, string username)
        {
            bool state = false;
            if (!string.IsNullOrWhiteSpace(configNameToAppend))
            {
                configNameToAppend = configNameToAppend.ToUpper();
                string configName = SystemConfigName.ENCRYPT_VALUE_CONFIGS.ToString();
                _dlObj = new DL.SystemConfigs(username);
                var obj = await _dlObj.GetAsync(configName).ConfigureAwait(false);
                if (obj != null)
                {
                    List<string> value = obj.Value != null ? obj.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    if (!value.Contains(configNameToAppend))
                    {
                        value.Add(configNameToAppend);
                        //
                        obj.Value = string.Join(",", value);
                        await _dlObj.UpdateAsync(obj).ConfigureAwait(false);
                    }
                }
                else
                {
                    var newObj = new SystemConfig
                    {
                        Created = DateTime.Now,
                        CreatedBy = username,
                        UniqueKey = Guid.NewGuid(),
                        Description = "",
                        Name = configName,
                        Value = configNameToAppend
                    };
                    await _dlObj.InsertAsync(newObj).ConfigureAwait(false);
                    return true;
                }
            }
            return state;
        }

        private async Task RemoveEncryptValueConfigAsync(Guid uniqueKey, string username)
        {
            _dlObj = new DL.SystemConfigs(username);
            var obj = await _dlObj.GetAsync(uniqueKey).ConfigureAwait(false);
            if (obj != null)
            {
                string configNameToRemove = obj.Name;
                string configName = SystemConfigName.ENCRYPT_VALUE_CONFIGS.ToString();
                obj = await _dlObj.GetAsync(configName).ConfigureAwait(false);
                if (obj != null)
                {
                    List<string> value = obj.Value != null ? obj.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    if (value.Count > 0 && value.Contains(configNameToRemove))
                    {
                        value.Remove(configNameToRemove);
                        //
                        obj.Value = string.Join(",", value);
                        await _dlObj.UpdateAsync(obj).ConfigureAwait(false);
                    }
                }
            }
        }

        #endregion

        public SystemConfigs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<SystemConfigCheckModel>> GetRequiredConfigsListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    return _dlObj.GetRequiredConfigsListAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.SystemConfig>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    return _dlObj.GetListAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.SystemConfig>> GetListAsync(IEnumerable<string> configNames)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    return _dlObj.GetListAsync(configNames);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

  
        public Task<Models.SystemConfig> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    return _dlObj.GetAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.SystemConfig> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    return _dlObj.GetAsync(uniqueKey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.SystemConfig> GetAsync(string name)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    return _dlObj.GetAsync(name);
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
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    var obj = await _dlObj.GetAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        string name = obj.Name;
                        string value = obj.Value;
                        if (autoDecrypt)
                            value = await TWC.IMS.Common.HelperClasses.SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name, value).ConfigureAwait(false);

                        return value;
                    }
                    else
                        throw new Exception($"No record found with id '{id}'");
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
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    var value = await _dlObj.GetValueAsync(name.ToString()).ConfigureAwait(false);
                    if (autoDecrypt)
                        value = await TWC.IMS.Common.HelperClasses.SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name.ToString(), value).ConfigureAwait(false);

                    return value;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<string> GetValueAsync(string name, bool autoDecrypt = true)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    var value = await _dlObj.GetValueAsync(name).ConfigureAwait(false);
                    if (autoDecrypt)
                        value = await TWC.IMS.Common.HelperClasses.SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, name.ToString(), value).ConfigureAwait(false);

                    return value;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> InsertAsync(Models.SystemConfig obj, bool encryptValue = false)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    if (encryptValue)
                    {
                        await UpdateEncryptValueConfigAsync(obj.Name, username).ConfigureAwait(false);
                    }
                    // encrypt value if necessary/configured
                    obj.Value = await TWC.IMS.Common.HelperClasses.SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(true, obj.Name, obj.Value).ConfigureAwait(false);
                    obj.UniqueKey = Guid.NewGuid();
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    using (_dlObj = new DL.SystemConfigs(username))
                    {
                        return await _dlObj.InsertAsync(obj).ConfigureAwait(false);
                    }
                }
                else throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateAsync(Models.SystemConfig obj, bool encryptValue = false)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    bool isInitial = false;
                    if (encryptValue)
                    {
                        isInitial = await UpdateEncryptValueConfigAsync(obj.Name, username).ConfigureAwait(false);
                    }
                    // encrypt value if necessary/configured
                    obj.Value = await TWC.IMS.Common.HelperClasses.SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(true, obj.Name, obj.Value).ConfigureAwait(false);
                    obj.Modified = DateTime.Now;
                    obj.ModifiedBy = username;

                    using (_dlObj = new DL.SystemConfigs(username))
                    {
                        return await _dlObj.UpdateAsync(obj).ConfigureAwait(false);
                    }
                }
                else throw new NullReferenceException();
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
                using (_dlObj = new DL.SystemConfigs(username))
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

        public async Task<int> DeleteAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    await RemoveEncryptValueConfigAsync(uniqueKey, username).ConfigureAwait(false);
                    return await _dlObj.DeleteAsync(uniqueKey).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> IsAutoLogoutEnabledAsync()
        {
            string strIsEnabled = await GetValueAsync(SystemConfigName.ENABLE_SIGNOUT_ALLUSERS_ON_PERMISSION_CHANGE).ConfigureAwait(false);
            bool isEnabled = false;
            bool.TryParse(strIsEnabled, out isEnabled);
            return isEnabled;
        }

        public async Task<DateTime> LockoutAccountDurationAsync(DateTime lockoutDate)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.SystemConfigs(username))
                {
                    var value = await _dlObj.GetValueAsync(SystemConfigName.LOCKED_ACCOUNT_DURATION_VALUE.ToString()).ConfigureAwait(false);
                    var unit = await _dlObj.GetValueAsync(SystemConfigName.LOCKED_ACCOUNT_DURATION_UNIT.ToString()).ConfigureAwait(false);
                    // default values
                    value = value ?? "30";
                    unit = unit ?? "DAYS";

                    switch (unit.ToUpper().Trim())
                    {
                        case "DAYS":
                            lockoutDate = lockoutDate.AddDays(Convert.ToDouble(value));
                            break;
                        case "MINUTES":
                            lockoutDate = lockoutDate.AddMinutes(Convert.ToDouble(value));
                            break;
                    }
                    return lockoutDate;
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
        // ~SystemConfigs() {
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
