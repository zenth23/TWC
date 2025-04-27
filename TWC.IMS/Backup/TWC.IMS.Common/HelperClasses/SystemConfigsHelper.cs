using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TWC.IMS.Common.DL;
using System.Diagnostics;

namespace TWC.IMS.Common.HelperClasses
{
    public static class SystemConfigsHelper
    {
        public static async Task<string> EncryptDecryptStringConfigValueAsync(bool isEncrypt, string configName, string configValue)
        {
            if (!string.IsNullOrEmpty(configValue))
            {
                var scDL = new SystemConfigs();
                var config = await scDL.GetEncryptValueConfigsValueAsync().ConfigureAwait(false);
                if (config != null)
                {
                    var configs = config.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < configs.Length; i++)
                    {
                        var name = configs[i].Trim();
                        // encrypt/decrypt value if configured
                        if (string.Compare(configName.Trim(), name, true) == 0)
                        {
                            if (isEncrypt)
                            {
                                // check if already encrypted
                                bool isEncrypted = true;
                                try
                                {
                                    byte[] x = Convert.FromBase64String(configValue);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                    isEncrypt = false;
                                }

                                if (!isEncrypted || configValue.Length % 4 != 0 || !configValue.EndsWith("="))
                                {
                                    // if has error, meaning the value is not encrypted yet
                                    byte[] encrypted = await TWC.IMS.Common.Cryptography.AESEncryptStringToBytesAsync(configValue).ConfigureAwait(false);
                                    string encryptedValue = Convert.ToBase64String(encrypted);
                                    return encryptedValue;
                                }
                            }
                            else
                            {
                                byte[] encrypted = Convert.FromBase64String(configValue);
                                string decryptedValue = await TWC.IMS.Common.Cryptography.AESDecryptStringToBytesAsync(encrypted).ConfigureAwait(false);
                                return decryptedValue;
                            }
                        }
                    }
                }
            }
            // return the original value
            return configValue;
        }
    }
}
