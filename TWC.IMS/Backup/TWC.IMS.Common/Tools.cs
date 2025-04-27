using Newtonsoft.Json;
using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using TWC.IMS.Common.DL;
using TWC.IMS.Common.HelperModels;
using System.Globalization;
using System.Net.Http;

namespace TWC.IMS.Common
{
    public class Tools
    {
        #region PRIVATE MEMBERS 
        private static Type GetNullableType(Type t)
        {
            Type returnType = t;
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                returnType = Nullable.GetUnderlyingType(t);
            }
            return returnType;
        }

        private static bool IsNullableType(Type type)
        {
            return (type == typeof(string) ||
                    type.IsArray ||
                    (type.IsGenericType &&
                     type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
        }

        private static string ToDisplayName(string name)
        {
            return name.Replace("_x_", "-").Replace("_", " ");
        }

        private static Task<bool> WaitForExitAsync(Process process, int timeout)
        {
            return Task.Run(() => process.WaitForExit(timeout));
        }

        #endregion

        public static Task<DataTable> ConvertListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            //Fill data into DataTable
            foreach (var item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return Task.FromResult(dataTable);
        }

        public static Task<DataTable> ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                var acc = info.GetAccessors().FirstOrDefault();

                if (acc.IsVirtual && !acc.IsFinal)
                {
                    Debug.WriteLine(string.Format("Prop Name: {0}", info.Name));
                }
                else
                {
                    if (info.PropertyType.Namespace == "System")
                    {
                        if (!dt.Columns.Contains(info.Name))
                            dt.Columns.Add(new DataColumn(ToDisplayName(info.Name), GetNullableType(info.PropertyType)));
                    }
                }
            }

            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                var properties = typeof(T).GetProperties().Where(a => dt.Columns.Contains(a.Name));
                foreach (PropertyInfo info in properties)
                {
                    var acc = info.GetAccessors().FirstOrDefault();

                    if (acc.IsVirtual && !acc.IsFinal)
                    {
                        Debug.WriteLine(string.Format("Val Prop Name: {0}", info.Name));
                    }
                    else
                    {
                        if (info.PropertyType.Namespace == "System")
                        {
                            if (!IsNullableType(info.PropertyType))
                                row[ToDisplayName(info.Name)] = info.GetValue(t, null);
                            else
                                row[ToDisplayName(info.Name)] = (info.GetValue(t, null) ?? DBNull.Value);
                        }

                        if (row[ToDisplayName(info.Name)] == DBNull.Value || row[ToDisplayName(info.Name)] == null)
                        {
                            var dupeProps = properties.Where(a => a.Name == info.Name).ToList();

                            foreach (var dp in dupeProps)
                            {
                                if (row[ToDisplayName(info.Name)] == DBNull.Value || row[ToDisplayName(info.Name)] == null)
                                {
                                    if (info.PropertyType.FullName.Contains("System.DateTimeOffset"))
                                    {
                                        object dateVal = null;

                                        if (!IsNullableType(info.PropertyType))
                                            dateVal = dp.GetValue(t, null);
                                        else
                                            dateVal = (dp.GetValue(t, null) ?? DBNull.Value);

                                        if (dateVal != null && dateVal != DBNull.Value)
                                        {
                                            var dateVal2 = DateTime.SpecifyKind(Convert.ToDateTime(dateVal), DateTimeKind.Local);
                                            try
                                            {
                                                DateTime dto = dateVal2;
                                                row[ToDisplayName(info.Name)] = dto;
                                            }
                                            catch (ArgumentException)
                                            {
                                                DateTimeOffset dto = dateVal2;
                                                row[ToDisplayName(info.Name)] = dto;
                                            }
                                        }
                                    }
                                    else if (info.PropertyType.Namespace == "System")
                                    {
                                        if (!IsNullableType(info.PropertyType))
                                            row[ToDisplayName(info.Name)] = dp.GetValue(t, null);
                                        else
                                            row[ToDisplayName(info.Name)] = (dp.GetValue(t, null) ?? DBNull.Value);
                                    }
                                }
                            }
                        }
                    }
                }
                dt.Rows.Add(row);
            }
            return Task.FromResult(dt);
        }

        public static async Task<byte[]> ZipFilesAsync(string path, List<string> fileNames, string fileExtension)
        {
            return await Task.Run(async () =>
            {
                using (var memoryStream = new MemoryStream())
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var fileName in fileNames)
                    {
                        string filePath = string.Format("{0}{1}.{2}", path, fileName, fileExtension);

                        var buffer = System.IO.File.ReadAllBytes(filePath);
                        var zipEntry = ziparchive.CreateEntry(string.Format("{0}.{1}", fileName, fileExtension));
                        //Get the stream
                        using (var originalFileStream = new MemoryStream(buffer))
                        {
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                //Copy the stream to the zip entry stream
                                await originalFileStream.CopyToAsync(zipEntryStream).ConfigureAwait(false);
                            }
                        }
                    }
                    return memoryStream.ToArray();
                }
            });
        }

        public static string ObjectToXml(object obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(obj.GetType());
            using (var sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, obj);
                return sww.ToString();
            }
        }

        public static string ObjectToJson(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return json;
        }

        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetBinaryFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                return await Task.Run(async () =>
                {
                    using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        await file.ReadAsync(bytes, 0, (int)file.Length).ConfigureAwait(false);
                        return bytes;
                    }
                });
            }
            else
                throw new FileNotFoundException($"GetBinaryFile Error: '{filePath}' does not exist.");
        }

        public static Task<bool> IsValidEmailAddress(string email)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(email.Trim()))
                {
                    var mail = new System.Net.Mail.MailAddress(email);
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// CPU-bound method. Run this method by wrapping in Task.Run()
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        public static Task<string> ConvertImageStreamToBase64(Stream imageStream)
        {
            return Task.Run(() =>
            {
                using (var image = Image.FromStream(imageStream, true, true))
                using (var m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            });
        }

        public static bool IsValidFileName(string fileName)
        {
            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            bool hasInvalidChars = fileName.Any(a => invalidFileNameChars.Contains(a));
            return !hasInvalidChars;
        }

        public static byte[] ReadBytes(string fileName, int buffer)
        {
            List<byte[]> dirtybytes = new List<byte[]>();

            using (BinaryReader b = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read)))
            {
                //hold position counters
                int pos = 0;
                int length = (int)b.BaseStream.Length;

                while (pos < length)
                {
                    //read the bytes
                    dirtybytes.Add(b.ReadBytes(buffer));

                    //increment the position
                    pos += buffer;
                }
            }
            //get the complete byte array 
            return dirtybytes.SelectMany(x => x).ToArray();
        }

        /// <summary>
        /// Sample output formats: Mar. 20 - 26, 2022 or Mar. 27 - Apr. 02, 2022
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static string ConvertDateRangeToDateRangeStringFormat(DateTime startDate, DateTime endDate)
        {
            // same month, same year
            if (startDate.Month == endDate.Month &&
                startDate.Year == endDate.Year)
            {
                return $"{startDate.ToString(TWC.IMS.Common.StringFormats.DATE_FORMAT_LONG_7)} - {endDate.ToString(TWC.IMS.Common.StringFormats.DATE_FORMAT_SHORT_10)}";
            }
            // not same month, same year
            else if (startDate.Month != endDate.Month &&
                     startDate.Year == endDate.Year)
            {
                return $"{startDate.ToString(TWC.IMS.Common.StringFormats.DATE_FORMAT_LONG_7)} - {endDate.ToString(TWC.IMS.Common.StringFormats.DATE_FORMAT_LONG_3)}";
            }
            else
            {
                // not same month, not same year
                return $"{startDate.ToString(TWC.IMS.Common.StringFormats.DATE_FORMAT_LONG_3)} - {endDate.ToString(TWC.IMS.Common.StringFormats.DATE_FORMAT_LONG_3)}";
            }
        }

        public static int GetWeekOfYear(DateTime date)
        {
            var woy = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Sunday);
            return woy;
        }

        public static DateTime[] GetStartEndDates(DateTime date)
        {
            int monthMaxDay = DateTime.DaysInMonth(date.Year, date.Month);
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = new DateTime(startDate.Year, startDate.Month, monthMaxDay);

            return new DateTime[] { startDate, endDate };
        }

        public static async Task<ProcessResult> RunProcessAsync(string exePath, string arguments)
        {
            int timeout = 5000; // 5 seconds
            var result = new ProcessResult();
            string workingDirectory = Path.GetDirectoryName(exePath);
            var procStartInfo = new ProcessStartInfo(exePath, arguments)
            {
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            using (var process = new Process { StartInfo = procStartInfo })
            {
                var outputBuilder = new StringBuilder();
                var outputCloseEvent = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (s, e) =>
                {
                    // The output stream has been closed i.e. the process has terminated
                    if (e.Data == null)
                    {
                        outputCloseEvent.SetResult(true);
                    }
                    else
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                var errorBuilder = new StringBuilder();
                var errorCloseEvent = new TaskCompletionSource<bool>();

                process.ErrorDataReceived += (s, e) =>
                {
                    // The error stream has been closed i.e. the process has terminated
                    if (e.Data == null)
                    {
                        errorCloseEvent.SetResult(true);
                    }
                    else
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                bool isStarted;

                try
                {
                    isStarted = process.Start();
                }
                catch (Exception error)
                {
                    // Usually it occurs when an executable file is not found or is not executable

                    result.Completed = true;
                    result.ExitCode = -1;
                    result.Output = error.Message;

                    isStarted = false;
                }

                if (isStarted)
                {
                    // Reads the output stream first and then waits because deadlocks are possible
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Creates task to wait for process exit using timeout
                    var waitForExit = WaitForExitAsync(process, timeout);

                    // Create task to wait for process exit and closing all output streams
                    var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

                    // Waits process completion and then checks it was not completed by timeout
                    if (await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result)
                    {
                        result.Completed = true;
                        result.ExitCode = process.ExitCode;

                        // Adds process output if it was completed with error
                        if (process.ExitCode != 0)
                        {
                            result.Output = $"{outputBuilder}{errorBuilder}";
                        }
                    }
                    else
                    {
                        try
                        {
                            // Kill hung process
                            process.Kill();
                        }
                        catch { }
                    }
                }
            }
            return result;
        }

        public static async Task RetryAsync(int numberOfRetries, Func<Task> action)
        {
            for (int i = 1; i <= numberOfRetries; i++)
            {
                try
                {
                    await action().ConfigureAwait(false);
                    break;
                }
                catch (Exception ex)
                {
                    string msg = $"Retry attempt {i} failed. ERROR: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}";
                    Console.WriteLine(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        /// <summary>
        /// Usage:
        /// Ascending : list.Sort((v1, v2) => Smits.Commom.Tools.CompareVersions(v1, v2));
        /// Descending: list.Sort((v1, v2) => Smits.Commom.Tools.CompareVersions(v2, v1));
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int CompareVersions(string v1, string v2)
        {
            if (v1 == "unknown" && v2 != "unknown") return 1;
            if (v1 != "unknown" && v2 == "unknown") return -1;
            if (v1 == "unknown" && v2 == "unknown") return 0;

            var parts1 = v1.Split('.');
            var parts2 = v2.Split('.');
            for (int i = 0; i < Math.Max(parts1.Length, parts2.Length); i++)
            {
                //int part1 = i < parts1.Length ? int.Parse(parts1[i]) : 0;
                //int part2 = i < parts2.Length ? int.Parse(parts2[i]) : 0;

                if (i >= parts1.Length) return -1;
                if (i >= parts2.Length) return 1;
                if (parts1[i] == parts2[i]) continue;
                if (parts1[i] == "unknown" && parts2[i] != "unknown") return 1;
                if (parts1[i] != "unknown" && parts2[i] == "unknown") return -1;

                int part1, part2;
                if (int.TryParse(parts1[i], out part1) && int.TryParse(parts2[i], out part2))
                {
                    if (part1 < part2) return -1;
                    if (part1 > part2) return 1;
                }
                else
                {
                    return string.Compare(parts1[i], parts2[i], StringComparison.OrdinalIgnoreCase);
                }
            }
            return 0;
        }
    }
}
