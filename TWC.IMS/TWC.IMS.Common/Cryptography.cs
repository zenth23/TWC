using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public class Cryptography
    {
        public static Task<string> Base64Decode(string base64EncodedData)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(base64EncodedData)) return string.Empty;

                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                string encoded = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                return encoded;
            });
        }

        public static Task<string> Base64Encode(string plainText)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(plainText)) return string.Empty;

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                var b64string = System.Convert.ToBase64String(plainTextBytes);
                return b64string;
            });
        }


        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="filePath">File path with file name and extension</param>
        /// <returns></returns>
        public static Task<string> GetFileSHA2512Hash(string filePath)
        {
            if (File.Exists(filePath))
            {
                return Task.Run(() =>
                {
                    using (var alg = SHA512.Create())
                    using (var stream = File.OpenRead(filePath))
                    {
                        alg.ComputeHash(stream);
                        var hash = BitConverter.ToString(alg.Hash).Replace("-", "").ToLowerInvariant();
                        return hash;
                    }
                });
            }
            else
                throw new FileNotFoundException($"GetSHA2512Hash Error: '{filePath}' does not exist.");
        }

        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Task<string> GetMD5Hash(string filePath)
        {
            if (File.Exists(filePath))
            {
                return Task.Run(() =>
                {
                    using (var alg = MD5.Create())
                    using (var stream = File.OpenRead(filePath))
                    {
                        var hash = alg.ComputeHash(stream);
                        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        return hashString;
                    }
                });
            }
            else
                throw new FileNotFoundException($"GetMD5Hash Error: '{filePath}' does not exist.");
        }

        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static Task<string> StringToSHA2512Hash(string word)
        {
            return Task.Run(() =>
            {
                using (var alg = SHA512.Create())
                {
                    // Convert the input string to a byte array and compute the hash.
                    byte[] data = alg.ComputeHash(Encoding.UTF8.GetBytes(word));

                    // Create a new Stringbuilder to collect the bytes
                    // and create a string.
                    StringBuilder sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data 
                    // and format each one as a hexadecimal string.
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    // Return the hexadecimal string.
                    return sBuilder.ToString();
                }
            });
        }

        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static Task<string> StringToMD5Hash(string word)
        {
            return Task.Run(() =>
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    // Convert the input string to a byte array and compute the hash.
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(word));

                    // Create a new Stringbuilder to collect the bytes
                    // and create a string.
                    StringBuilder sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data 
                    // and format each one as a hexadecimal string.
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    // Return the hexadecimal string.
                    return sBuilder.ToString();
                }
            });
        }

        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged?redirectedfrom=MSDN&view=netframework-4.8
        public static async Task<byte[]> AESEncryptStringToBytesAsync(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            return await Task.Run(async () =>
            {
                byte[] encrypted;
                // Create an AesManaged object
                // with the specified key and IV.
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.KeySize = 128;              // in bits
                    aesAlg.Key = new byte[128 / 8];    // 16 bytes for 128 bits encryption
                    aesAlg.IV = new byte[128 / 8];     // AES needs a 16-byte IV

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                await swEncrypt.WriteAsync(plainText).ConfigureAwait(false);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                // Return the encrypted bytes from the memory stream.
                return encrypted;
            });
        }

        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static async Task<string> AESDecryptStringToBytesAsync(byte[] cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            return await Task.Run(async () =>
            {
                // Declare the string used to hold
                // the decrypted text.
                string plaintext = null;

                // Create an AesManaged object
                // with the specified key and IV.
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.KeySize = 128;              // in bits
                    aesAlg.Key = new byte[128 / 8];    // 16 bytes for 128 bits encryption
                    aesAlg.IV = new byte[128 / 8];     // AES needs a 16-byte IV

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = await srDecrypt.ReadToEndAsync().ConfigureAwait(false);
                            }
                        }
                    }
                }

                return plaintext;
            });
        }

        /// <summary>
        /// Returns a base-64 format string. Format: Basic {encrypted credentials}
        /// Usage: httpClient.DefaultRequestHeaders.Add("Authorization", credBase64String);
        /// </summary>
        /// <returns></returns>
        public static string EncryptBasicAuthenticationCredentials(string username, string password)
        {
            string creds = $"{username}:{password}";
            var credBytes = Encoding.UTF8.GetBytes(creds);
            string credBase64String = Convert.ToBase64String(credBytes);
            return $"Basic {credBase64String}";
        }

        public static string GetSha256CheckSum(string filePath)
        {
            using (SHA256 SHA256 = SHA256Managed.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return BitConverter.ToString(SHA256.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
