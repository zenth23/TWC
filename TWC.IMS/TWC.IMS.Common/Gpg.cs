using TWC.IMS.Common.HelperModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class Gpg
    {
        public static async Task<ProcessResult> DecryptFileAsync(string fullInputFilename)
        {
            try
            {
                using (var scDL = new DL.SystemConfigs())
                {
                    string outputDir = await scDL.GetValueAsync(HelperClasses.SystemConfigName.DOWNLOAD_DEC_DIR).ConfigureAwait(false);
                    string gpgHomeDir = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_HOME_DIR).ConfigureAwait(false);
                    string sapGpgUser_hr = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_SAP_USERID_HR).ConfigureAwait(false);
                    string sapGpgUser_smpi = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_SAP_USERID_SMPI).ConfigureAwait(false);
                    string tmsGpgUserPassPhrase = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_TMS_PASSPHRASE, true).ConfigureAwait(false);

                    // format
                    outputDir = string.Format(outputDir, DateTime.Now);

                    // create directories
                    Directory.CreateDirectory(outputDir);

                    string filename = Path.GetFileName(fullInputFilename);
                    string fullOutputFilename = Path.Combine(outputDir, $"{filename}");
                    // with replace
                    string arguments = $"--homedir {gpgHomeDir} --batch --yes --passphrase {tmsGpgUserPassPhrase} --pinentry-mode loopback --output \"{fullOutputFilename}\" --decrypt \"{fullInputFilename}\"";
                    string path = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_EXE_FILE_PATH).ConfigureAwait(false);

                    var result = await TWC.IMS.Common.Tools.RunProcessAsync(path, arguments).ConfigureAwait(false);
                    // if successful, return decrypted filename as output
                    if (result.Completed && result.ExitCode == 0)
                    {
                        result.Output = fullOutputFilename;

                        string sapGpgUser = sapGpgUser_smpi;
                        if (filename.StartsWith("emp_", StringComparison.OrdinalIgnoreCase) &&
                            filename.StartsWith("hol_", StringComparison.OrdinalIgnoreCase))
                        {
                            sapGpgUser = sapGpgUser_hr;
                        }
                        result.GpgUserId = sapGpgUser;
                    }
                    else
                    {
                        // if failed but file was decrypted
                        // do some cleanup
                        if (File.Exists(fullOutputFilename))
                        {
                            File.Delete(fullOutputFilename);
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static async Task<ProcessResult> EncryptFileAsync(string fullInputFilename)
        {
            try
            {
                using (var scDL = new DL.SystemConfigs())
                {
                    string outputDir = await scDL.GetValueAsync(HelperClasses.SystemConfigName.BACKUP_UPLOAD_ENC_DIR).ConfigureAwait(false);
                    string passPhrase = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_TMS_PASSPHRASE, true).ConfigureAwait(false);
                    string gpgHomeDir = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_HOME_DIR).ConfigureAwait(false);
                    string tmsGpgUser = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_TMS_USERID).ConfigureAwait(false);
                    string sapGpgUser = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_SAP_USERID_HR).ConfigureAwait(false);

                    // format
                    outputDir = string.Format(outputDir, DateTime.Now);

                    string fullOutputFilename = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(fullInputFilename)}.csv");
                    // with replace
                    string arguments = $"--homedir {gpgHomeDir} --batch --yes --passphrase {passPhrase} --pinentry-mode loopback -a --force-mdc --cipher-algo AES256 -r {sapGpgUser} -r {tmsGpgUser} --output \"{fullOutputFilename}\" --sign --encrypt \"{fullInputFilename}\"";
                    string path = await scDL.GetValueAsync(HelperClasses.SystemConfigName.GPG_EXE_FILE_PATH).ConfigureAwait(false);

                    var result = await TWC.IMS.Common.Tools.RunProcessAsync(path, arguments).ConfigureAwait(false);
                    if (result.Completed && result.ExitCode == 0)
                        result.Output = fullOutputFilename;

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
