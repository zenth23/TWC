using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class FileStreamUpload
    {
        /// <summary>
        /// Transfers file in chunks. Ideal for server-to-server, local-to-remote, over-the-network big file transfers.
        /// </summary>
        /// <param name="sourceFilename">File to be copied. Full path with file name and extension</param>
        /// <param name="destinationFileName">Full path with file name and extension</param>
        /// <param name="replaceDestinationFileIfExists"></param>
        /// <param name="maxFileSizeInKB">Default, 100KB per transfer</param>
        /// <param name="transferDelayInMS">Default, 10 milliseconds (ms). Minimum delay, 0ms. Maximum delay, 100ms</param>
        /// <returns></returns>
        public static async Task<FileInfo> UploadFileChunkAsync(string sourceFilename, string destinationFileName, bool replaceDestinationFileIfExists = false, int maxFileSizeInKB = 100, int transferDelayInMS = 10)
        {
            if (File.Exists(destinationFileName) && replaceDestinationFileIfExists)
            {
                File.Delete(destinationFileName);
            }
            else if (File.Exists(destinationFileName) && !replaceDestinationFileIfExists)
            {
                string path = Path.GetDirectoryName(destinationFileName);
                string fileExt = Path.GetExtension(destinationFileName);
                string newFilename = $"{Path.GetFileNameWithoutExtension(destinationFileName)}_{DateTime.Now.Ticks}{fileExt}";
                destinationFileName = Path.Combine(path, newFilename);
            }
            // control for delay
            transferDelayInMS = transferDelayInMS > 100 ? 100 : transferDelayInMS < 0 ? 0 : transferDelayInMS;

            int buffer = maxFileSizeInKB * 1024;
            using (BinaryReader b = new BinaryReader(File.Open(sourceFilename, FileMode.Open, FileAccess.Read)))
            {
                // hold position counters
                int pos = 0;
                int length = (int)b.BaseStream.Length;
                while (pos < length)
                {
                    using (FileStream fs = new FileStream(destinationFileName, FileMode.Append))
                    {
                        //read the bytes
                        var bytes = b.ReadBytes(buffer);
                        // then write file chunk to disk
                        await fs.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                        // beneficial for network transfers 
                        await Task.Delay(transferDelayInMS).ConfigureAwait(false);
                    }
                    // increment the position
                    pos += buffer;
                }
            }

            return new FileInfo(destinationFileName);
        }

        public static byte[] GetStreamBytes(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
