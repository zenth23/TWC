using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Async;
using TWC.IMS.Common.HelperClasses;
using System.IO;
using TWC.IMS.Common.DL;
using Renci.SshNet.Sftp;
using System.Diagnostics;

namespace TWC.IMS.Common.SftpHelper
{
    public class Sftp
    {
        private string _password;
        private string _username;
        private string _host;
        private int _port;

        #region PRIVATE MEMBERS
        private void CreateFolder(SftpClient client, string filePath)
        {
            if (!client.Exists(filePath))
            {
                string path = "/"; //root
                var dirList = filePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string dir in dirList)
                {
                    string folder = dir.Trim();
                    path = $"{path}/{folder}".ToUpper();
                    if (!client.Exists(path))
                    {
                        client.CreateDirectory(folder);
                    }
                    client.ChangeDirectory(path);
                }
            }
        }

        #endregion

        public Sftp() { }

        public Sftp(string host, string username, string password, int port)
        {
            _host = host;
            _username = username;
            _password = password;
            _port = port;
        }

        public async Task InitAsync()
        {
            if (string.IsNullOrEmpty(_host) && string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password) && _port == 0)
            {
                var scBL = new SystemConfigs();
                var configs = await scBL.GetListAsync().ConfigureAwait(false);
                var objHost = configs.FirstOrDefault(a => string.Compare(a.Name, SftpConfigName.SFTP_HOST.ToString(), true) == 0);
                var objUsername = configs.FirstOrDefault(a => string.Compare(a.Name, SftpConfigName.SFTP_USERNAME.ToString(), true) == 0);
                var objPassword = configs.FirstOrDefault(a => string.Compare(a.Name, SftpConfigName.SFTP_PASSWORD.ToString(), true) == 0);
                var objPort = configs.FirstOrDefault(a => string.Compare(a.Name, SftpConfigName.SFTP_PORT.ToString(), true) == 0);

                if (objHost != null)
                    _host = objHost.Value;
                else
                    throw new Exception("SFTP host not defined.");

                if (objUsername != null)
                    _username = objUsername.Value;
                else
                    throw new Exception("SFTP username not defined.");

                if (objPassword != null)
                    _password = await SystemConfigsHelper.EncryptDecryptStringConfigValueAsync(false, SftpConfigName.SFTP_PASSWORD.ToString(), objPassword.Value);
                else
                    throw new Exception("SFTP password not defined.");

                if (objPort != null)
                    _port = Convert.ToInt32(objPort.Value);
                else
                    throw new Exception("SFTP port number not defined.");
            }
        }

        public async Task<string> UploadFileAsync(string sftpFullPath, byte[] file, string fileNameWithExtension, string destinationFilenameWithExtenstion, bool overwriteFileIfExists = false)
        {
            // assumption: sftp account has read, write permissions
            string fileExtention = Path.GetExtension(destinationFilenameWithExtenstion);
            if (!Path.HasExtension(destinationFilenameWithExtenstion))
            {
                fileExtention = Path.GetExtension(fileNameWithExtension);
                destinationFilenameWithExtenstion = $"{destinationFilenameWithExtenstion}.{fileExtention}";
            }

            var isValid = Tools.IsValidFileName(destinationFilenameWithExtenstion);
            if (!isValid)
                throw new Exception("Path and/or file names arguments have invalid characters.");

            try
            {
                using (SftpClient client = new SftpClient(_host, _port, _username, _password))
                {
                    client.Connect();
                    CreateFolder(client, sftpFullPath);
                    client.ChangeDirectory(sftpFullPath);

                    using (Stream fs = new MemoryStream(file))
                    {
                        sftpFullPath = $"{sftpFullPath}/{destinationFilenameWithExtenstion}";
                        if (client.Exists(sftpFullPath) && overwriteFileIfExists)
                            client.DeleteFile(sftpFullPath);

                        await client.UploadAsync(fs, sftpFullPath).ConfigureAwait(false);
                        return sftpFullPath;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<string>> DownloadFilesAsync(string sftpFullPath, string fileExtensionFilter, string destinationFullPath)
        {
            try
            {
                using (SftpClient client = new SftpClient(_host, _port, _username, _password))
                {
                    client.Connect();
                    this.ChangeDirectory(client, sftpFullPath);

                    if (string.Compare(client.WorkingDirectory, sftpFullPath, true) == 0)
                    {
                        var list = new HashSet<string>();
                        var files = await client.ListDirectoryAsync(sftpFullPath).ConfigureAwait(false);
                        files = files.Where(a => a.Name.EndsWith(fileExtensionFilter) &&
                                                 !a.IsDirectory);
                        foreach (var file in files)
                        {
                            string filename = file.Name;
                            string destinationFile = Path.Combine(destinationFullPath, filename);
                            if (File.Exists(destinationFile))
                            {
                                // then delete
                                File.Delete(destinationFile);
                            }

                            // create new
                            using (Stream fs = File.Create(destinationFile))
                            {
                                string sourceFile = sftpFullPath + "/" + filename;
                                await client.DownloadAsync(sourceFile, fs).ConfigureAwait(false);

                                list.Add(destinationFile);

                                // then delete file from source
                                await Task.Run(() => client.DeleteFile(sourceFile)).ConfigureAwait(false);
                            }
                        }
                        return list;
                    }
                    else
                        throw new DirectoryNotFoundException($"Unable to reach {sftpFullPath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public void CreateSftpFolder(string path)
        {
            using (SftpClient client = new SftpClient(_host, _port, _username, _password))
            {
                client.Connect();
                CreateFolder(client, path);
            }
        }

        public void ChangeDirectory(SftpClient client, string filePath)
        {
            string path = "/";
            var dirList = filePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string dir in dirList)
            {
                string folder = dir.Trim();
                path = $"{path}/{folder}".ToUpper();
                if (client.Exists(path))
                {
                    client.ChangeDirectory(path);
                }
            }
        }
    }

    public class SftpFactory
    {
        public static async Task<Sftp> SftpA()
        {
            var obj = new Sftp();
            await obj.InitAsync().ConfigureAwait(false);
            return obj;
        }

        public static async Task<Sftp> SftpA(string host, string username, string password, int port)
        {
            var obj = new Sftp(host, username, password, port);
            await obj.InitAsync().ConfigureAwait(false);
            return obj;
        }
    }
}
