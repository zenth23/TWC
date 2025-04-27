using TWC.IMS.Common.HelperClasses;
using TWC.IMS;
using TWC.IMS.Models.ChartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class ErrorLogs : IDisposable
    {
        private DL.ErrorLogs _dlObj = null;
        private string _username;

        public ErrorLogs(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        #region Chart Queries
        /// <summary>
        /// Month by day
        /// </summary>
        /// <param name="date"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ChartModels.ErrorFrequencyModel>> GetErrorFrequencyDataAsync(DateTime date, IEnumerable<Models.ErrorLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    int year = date.Year;
                    int month = date.Month;
                    int startDay = 1;
                    int endDay = DateTime.DaysInMonth(year, month);
                    DateTime startDate = new DateTime(year, month, startDay);
                    DateTime endDate = new DateTime(year, month, endDay);

                    // create date range
                    var days = Enumerable.Range(0, endDay).Select(d => startDate.AddDays(d)).ToList();

                    if (list == null)
                    {
                        list = await _dlObj.GetListAsync(date).ConfigureAwait(false);
                        list = list.Where(a => a.Created.HasValue);
                    }
                    // left join
                    return days.GroupJoin(list,
                                          left => left.Date,
                                          right => right.Created.Value.Date,
                                          (l, r) => new
                                          {
                                              Date = l.Date,
                                              Data = r
                                          })
                                          .SelectMany(x => x.Data.DefaultIfEmpty(),
                                          (l, r) => new Models.ChartModels.ErrorFrequencyModel
                                          {
                                              Timestamp = l.Date,
                                              ErrorCount = l.Data?.Count(a => a.MessageType == MessageType.ERROR.ToString()) ?? 0,
                                              InformationCount = l.Data?.Count(a => a.MessageType == MessageType.INFORMATION.ToString()) ?? 0,
                                              WarningCount = l.Data?.Count(a => a.MessageType == MessageType.WARNING.ToString()) ?? 0
                                          });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.ChartModels.ErrorRateCvpModel>> GetErrorRateChartDataCvpAsync(DateTime currDate, DateTime prevDate)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    var list = new List<Models.ErrorLog>();
                    var currList = await _dlObj.GetListAsync(currDate).ConfigureAwait(false);
                    currList = currList.Where(a => a.Created.HasValue &&
                                                   a.MessageType == MessageType.ERROR.ToString());
                    list.AddRange(currList);

                    var prevList = await _dlObj.GetListAsync(prevDate).ConfigureAwait(false);
                    prevList = prevList.Where(a => a.Created.HasValue &&
                                                   a.MessageType == MessageType.ERROR.ToString());
                    list.AddRange(prevList);

                    // days
                    var days = Enumerable.Range(1, 31);
                    // left join
                    var data = days.GroupJoin(list,
                                              left => left,
                                              right => right.Created.Value.Date.Day,
                                              (l, r) => new
                                              {
                                                  Day = l,
                                                  Data = r
                                              })
                                              .SelectMany(x => x.Data.DefaultIfEmpty(),
                                              (l, r) => new
                                              {
                                                  Data = l.Data,
                                                  Day = l.Day
                                              })
                                              .GroupBy(a => new { a.Day })
                                              .Select(a => new Models.ChartModels.ErrorRateCvpModel
                                              {
                                                  Day = a.Key.Day,
                                                  RateCurrent = a.Select(b => b.Data.Count(c => c.Created.Value.Month == currDate.Month)).FirstOrDefault(),
                                                  RatePrevious = a.Select(b => b.Data.Count(c => c.Created.Value.Month == prevDate.Month)).FirstOrDefault()
                                              })
                                              .ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Day by hour
        /// </summary>
        /// <param name="date"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ChartModels.ErrorTrendModel>> GetErrorTrendChartDataAsync(DateTime date)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    // create date range
                    var hours = Enumerable.Range(0, 24);

                    var list = await _dlObj.GetListByDayAsync(date).ConfigureAwait(false);
                    list = list.Where(a => a.Created.HasValue);
                    // left join
                    var flist = hours.GroupJoin(list,
                                                left => left,
                                                right => right.Created.Value.Hour,
                                                (l, r) => new
                                                {
                                                    Hour = l,
                                                    Data = r
                                                })
                                                .SelectMany(x => x.Data.DefaultIfEmpty(),
                                                (l, r) => new
                                                {
                                                    Hour = l.Hour,
                                                    Data = l.Data
                                                })
                                                .GroupBy(a => a.Hour)
                                                .Select(a => new Models.ChartModels.ErrorTrendModel
                                                {
                                                    Hour = a.Key,
                                                    ErrorCount = a.Select(b => b.Data.Count()).FirstOrDefault()
                                                });
                    return flist;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.ChartModels.PageHitsModel>> GetMethodHitCountListAsync(DateTime date)
        {
            try
            {
                using (_dlObj = new DL.ErrorLogs(_username))
                {
                    return _dlObj.GetMethodHitCountListAsync(date.Month, date.Year, 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// By month, top 4
        /// </summary>
        /// <param name="date"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<Models.ChartModels.AdminDashboardCounterModel> GetLogCountersAsync(DateTime date, IEnumerable<Models.ErrorLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    if (list == null)
                    {
                        list = await _dlObj.GetListAsync(date).ConfigureAwait(false);
                        list = list.Where(a => a.Created.HasValue);
                    }

                    int totalErrors = list.Count();
                    int totalLoggedUsers = list.Select(a => new { a.CreatedBy }).Distinct().Count();

                    var errorCountByMethodList = list.GroupBy(a => a.MethodName)
                    .Select(a => new
                    {
                        MethodName = a.Key,
                        Count = a.Count()
                    })
                    .OrderByDescending(a => a.Count)
                    .Take(4)
                    .Select(a => new ErrorCountByMethodModel
                    {
                        MethodName = a.MethodName,
                        ErrorCount = a.Count
                    });

                    return new Models.ChartModels.AdminDashboardCounterModel
                    {
                        TotalErrors = totalErrors,
                        TotalLoggedUsers = totalLoggedUsers,
                        ErrorCountByMethodList = errorCountByMethodList
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// By month, top 4
        /// </summary>
        /// <param name="date"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ChartModels.ApplicationVersionErrorDistributionModel>> GetApplicationVersionErrorDistributionChartDataAsync(DateTime date, IEnumerable<Models.ErrorLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    if (list == null)
                    {
                        list = await _dlObj.GetListAsync(date).ConfigureAwait(false);
                        list = list.Where(a => a.Created.HasValue);
                    }
                    var flist = list.GroupBy(a => a.AppVersion)
                    .Select(a => new Models.ChartModels.ApplicationVersionErrorDistributionModel
                    {
                        AppVersion = a.Key ?? "Unknown",
                        ErrorCount = a.Count()
                    })
                    .ToList();
                    // sort descending
                    flist.Sort((v1, v2) => TWC.IMS.Common.Tools.CompareVersions(v2.AppVersion, v1.AppVersion));
                    return flist.Take(4);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// By month
        /// </summary>
        /// <param name="date"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ChartModels.UserRelatedMetricsModel>> GetUserRelatedMetricsChartDataAsync(DateTime date, IEnumerable<Models.ErrorLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    if (list == null)
                    {
                        list = await _dlObj.GetListAsync(date).ConfigureAwait(false);
                        list = list.Where(a => a.Created.HasValue);
                    }
                    return list.GroupBy(a => new { a.CreatedBy, a.UserRole, a.ClientIPAddress, a.AppVersion })
                               .Select(a => new Models.ChartModels.UserRelatedMetricsModel
                               {
                                   ProfileThumbnail = a.Key.CreatedBy.Trim().Substring(0, 1).ToUpper(),
                                   Username = a.Key.CreatedBy,
                                   IPAddress = a.Key.ClientIPAddress,
                                   Role = a.Key.UserRole,
                                   AppVersion = a.Key.AppVersion ?? "Unknown",
                                   ErrorCount = a.Count()
                               })
                               .OrderByDescending(a => a.ErrorCount);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Year by month
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ChartModels.ErrorRateModel>> GetErrorRateChartDataAsync(int year)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    var list = await _dlObj.GetListAsync(year).ConfigureAwait(false);
                    list = list.Where(a => a.Created.HasValue);

                    // months
                    var months = Enumerable.Range(1, 12);
                    // left join
                    var data = months.GroupJoin(list,
                                            left => left,
                                            right => right.Created.Value.Date.Month,
                                            (l, r) => new
                                            {
                                                Month = l,
                                                Data = r
                                            })
                                            .SelectMany(x => x.Data.DefaultIfEmpty(),
                                            (l, r) => new
                                            {
                                                Data = l.Data,
                                                Month = l.Month,
                                                MonthString = (new DateTime(2023, l.Month, 1)).ToString("MMM")
                                            })
                                            .GroupBy(a => new { a.Month, a.MonthString })
                                            .Select(a => new Models.ChartModels.ErrorRateModel
                                            {
                                                Month = a.Key.Month,
                                                MonthString = a.Key.MonthString,
                                                TotalLogCount = a.Select(b => b.Data.Count()).FirstOrDefault(),
                                                TotalErrorCount = a.Select(b => b.Data?.Count(c => c.MessageType == MessageType.ERROR.ToString())).FirstOrDefault() ?? 0
                                            })
                                            .ToList();

                    data.Select(a =>
                    {
                        a.Rate = a.TotalLogCount == 0 ? 0 : (a.TotalErrorCount / (double)a.TotalLogCount) * 100;
                        return a;
                    })
                    .ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// By month
        /// </summary>
        /// <param name="date"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ChartModels.ErrorSeverityModel>> GetErrorSeverityChartDataAsync(DateTime date, IEnumerable<Models.ErrorLog> list = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    if (list == null)
                    {
                        list = await _dlObj.GetListAsync(date).ConfigureAwait(false);
                        list = list.Where(a => a.Created.HasValue);
                    }
                    return list.GroupBy(a => a.ImpactLevel).Select(a => new Models.ChartModels.ErrorSeverityModel
                    {
                        ImpactLevel = a.Key ?? "Unknown",
                        ErrorCount = a.Count()
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        public Task<IEnumerable<Models.ErrorLog>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
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

        public Task<IEnumerable<Models.ErrorLog>> GetListAsync(DateTime date)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
                {
                    return _dlObj.GetListAsync(date);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.ErrorLog> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
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

        public Task<Models.ErrorLog> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.ErrorLogs(username))
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
        // ~ErrorLogs() {
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
