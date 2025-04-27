using Newtonsoft.Json;
using TWC.IMS;
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
    public class DemoSources : IDisposable
    {
        private string _username;

        private const string _TREEMAP_CHART_NAME = "License Subscriptions";

        public DemoSources(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        #region Chart Queries

        public async Task<IEnumerable<Models.ChartModels.BarChartModel>> GetCurrentMonthTop5LicenseSubscriptionsAsync(DateTime date, bool includeMaintenance = false, bool approvedOnly = true)
        {
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.BarChartModel>
                {
                    new Models.ChartModels.BarChartModel { Category = "A", Currency = "PHP", Value = 123456  },
                    new Models.ChartModels.BarChartModel { Category = "B", Currency = "USD", Value = 4567890  },
                    new Models.ChartModels.BarChartModel { Category = "C", Currency = "PHP", Value = 4647878  },
                    new Models.ChartModels.BarChartModel { Category = "D", Currency = "USD", Value = 23423  },
                    new Models.ChartModels.BarChartModel { Category = "E", Currency = "USD", Value = 34633  },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.ChartModels.BarChartModel>> GetCurrentMonthTop5BUSubscriptionsAsync(DateTime date, bool includeMaintenance = false, bool approvedOnly = true)
        {
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.BarChartModel>
                {
                    new Models.ChartModels.BarChartModel { Category = "A", Currency = "PHP", Value = 0, Percentage = 99.8M  },
                    new Models.ChartModels.BarChartModel { Category = "B", Currency = "USD", Value = 0, Percentage = 56M  },
                    new Models.ChartModels.BarChartModel { Category = "C", Currency = "PHP", Value = 0, Percentage = 22M  },
                    new Models.ChartModels.BarChartModel { Category = "D", Currency = "USD", Value = 0, Percentage = 86.1M  },
                    new Models.ChartModels.BarChartModel { Category = "E", Currency = "USD", Value = 0, Percentage = 45.3M  },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.ChartModels.TreeMapChartModel>> GetCurrentMonthLicenseQuantitiesPerBUAsync(DateTime date, bool includeMaintenance = false, bool approvedOnly = true)
        {
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.TreeMapChartModel> {
                    new Models.ChartModels.TreeMapChartModel
                    {
                        Name = _TREEMAP_CHART_NAME,
                        Value = 567890099,
                        Items = new List<Models.ChartModels.TreeMapChartModel> {
                            new Models.ChartModels.TreeMapChartModel { Name = "A", Value = 45678, Items = null },
                            new Models.ChartModels.TreeMapChartModel { Name = "B", Value = 345345, Items = null },
                            new Models.ChartModels.TreeMapChartModel { Name = "C", Value = 2332, Items = null },
                            new Models.ChartModels.TreeMapChartModel { Name = "D", Value = 3545, Items = null },
                            new Models.ChartModels.TreeMapChartModel { Name = "E", Value = 234, Items = null },
                            new Models.ChartModels.TreeMapChartModel { Name = "F", Value = 4546, Items = null },
                            new Models.ChartModels.TreeMapChartModel { Name = "G", Value = 868, Items = null },
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.ChartModels.LineChartModel>> GetTopNSumofLicensesQuantitiesAsync(int year, bool includeMaintenance = false, bool approvedOnly = true, bool isTop5 = true)
        {
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.LineChartModel>
                {
                    new Models.ChartModels.LineChartModel {  Category = "A", Duration = new DateTime(2023, 1, 1), Value = 1.1M },
                    new Models.ChartModels.LineChartModel {  Category = "B", Duration = new DateTime(2023, 2, 2), Value = 2M },
                    new Models.ChartModels.LineChartModel {  Category = "C", Duration = new DateTime(2023, 3, 3), Value = 3M },
                    new Models.ChartModels.LineChartModel {  Category = "D", Duration = new DateTime(2023, 4, 4), Value = 4M },
                    new Models.ChartModels.LineChartModel {  Category = "E", Duration = new DateTime(2023, 5, 5), Value = 5M },
                    new Models.ChartModels.LineChartModel {  Category = "F", Duration = new DateTime(2023, 6, 6), Value = 6M },
                    new Models.ChartModels.LineChartModel {  Category = "G", Duration = new DateTime(2023, 7, 7), Value = 7M },
                    new Models.ChartModels.LineChartModel {  Category = "H", Duration = new DateTime(2023, 8, 8), Value = 8M },
                    new Models.ChartModels.LineChartModel {  Category = "I", Duration = new DateTime(2023, 9, 9), Value = 9M },
                    new Models.ChartModels.LineChartModel {  Category = "J", Duration = new DateTime(2023, 10, 10), Value = 10M },
                    new Models.ChartModels.LineChartModel {  Category = "K", Duration = new DateTime(2023, 11, 11), Value = 11M },
                    new Models.ChartModels.LineChartModel {  Category = "L", Duration = new DateTime(2023, 12, 12), Value = 12M },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

        #region TotalSalesReport
        public async Task<IEnumerable<Models.ChartModels.TotalSalesReportModel>> DashboardTotalSalesReportAsync(int year, int month)
        {
            string username = _username;
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.TotalSalesReportModel> {
                    new Models.ChartModels.TotalSalesReportModel
                    {
                        CurrentYear = 2023,
                        LicenseCurrencyCode = "PHP",
                        Percentage = 100,
                        PreviousYear = 2022,
                        TotalAmountCurrentYear = 123456,
                        TotalAmountPreviousYear = 0
                    },
                    new Models.ChartModels.TotalSalesReportModel
                    {
                        CurrentYear = 2023,
                        LicenseCurrencyCode = "USD",
                        Percentage = -12,
                        PreviousYear = 2022,
                        TotalAmountCurrentYear = 3534,
                        TotalAmountPreviousYear = 4565
                    },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region AverageRevenuePerUnitReport
        public async Task<IEnumerable<Models.ChartModels.AverageRevenuePerUnitReportModel>> DashboardAverageRevenuePerUnitReportAsync(int year, int month)
        {
            string username = _username;
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.AverageRevenuePerUnitReportModel>
                {
                    new Models.ChartModels.AverageRevenuePerUnitReportModel {
                        Duration = "June",
                        LicenseCurrencyCode = "PHP",
                        TotalBusinessUnits = 100,
                        TotalRevenue = 1234567890
                    },
                    new Models.ChartModels.AverageRevenuePerUnitReportModel {
                        Duration = "June",
                        LicenseCurrencyCode = "USD",
                        TotalBusinessUnits = 100,
                        TotalRevenue = 2343223
                    },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region BillingProgress
        public async Task<IEnumerable<Models.ChartModels.BillingProgressReportModel>> DashboardBillingProgressAsync(int year, int month)
        {
            string username = _username;
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.BillingProgressReportModel>
                {
                    new Models.ChartModels.BillingProgressReportModel {
                        Category = "Unbilled",
                        PreviousMonth = "May",
                        TotalCount = 100,
                        Value = 15
                    },
                    new Models.ChartModels.BillingProgressReportModel {
                        Category = "For Approval",
                        PreviousMonth = "May",
                        TotalCount = 100,
                        Value = 25
                    },
                    new Models.ChartModels.BillingProgressReportModel {
                        Category = "Approved",
                        PreviousMonth = "May",
                        TotalCount = 100,
                        Value = 60
                    },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        public async Task<IEnumerable<Models.ChartModels.BillingRequestsBillingProgressModel>> GetBillingProgressDetailsDataAsync(int year, int month, string statusCode)
        {
            string username = _username;
            try
            {
                await Task.Delay(0);
                var list = new List<Models.ChartModels.BillingRequestsBillingProgressModel>
                {
                    new Models.ChartModels.BillingRequestsBillingProgressModel {
                        BRReferenceNo = "10001",
                        BusinessUnitCode = "A",
                        BusinessUnitName = "Aaa",
                        Modified = DateTime.Now,
                        ModifiedBy = "admin",
                        StatusCode = "APRV",
                        StatusName = "Approved"
                    },
                    new Models.ChartModels.BillingRequestsBillingProgressModel {
                        BRReferenceNo = "10002",
                        BusinessUnitCode = "A",
                        BusinessUnitName = "Aaa",
                        Modified = DateTime.Now,
                        ModifiedBy = "admin",
                        StatusCode = "APRV",
                        StatusName = "Approved"
                    },
                    new Models.ChartModels.BillingRequestsBillingProgressModel {
                        BRReferenceNo = "10003",
                        BusinessUnitCode = "B",
                        BusinessUnitName = "Bbb",
                        Modified = DateTime.Now,
                        ModifiedBy = "admin",
                        StatusCode = "FRAPRV",
                        StatusName = "For Approval"
                    },
                    new Models.ChartModels.BillingRequestsBillingProgressModel {
                        BRReferenceNo = "10004",
                        BusinessUnitCode = "C",
                        BusinessUnitName = "Ccc",
                        Modified = DateTime.Now,
                        ModifiedBy = "admin",
                        StatusCode = "FRAPRV",
                        StatusName = "For Approval"
                    },
                };
                return list.Where(a => string.Compare(a.StatusCode, statusCode, true) == 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.ChartModels.YearlySalesReportModel>> DashboardYearlyReportAsync(string subscriptionTypeCode, int year)
        {
            string username = _username;
            try
            {
                await Task.Delay(0);
                return new List<Models.ChartModels.YearlySalesReportModel>
                {
                    new Models.ChartModels.YearlySalesReportModel {
                        January = 1,
                        February = 2,
                        March = 3,
                        April = 4,
                        May = 5,
                        June = 6,
                        July = 7,
                        August = 8,
                        September = 9,
                        October = 10,
                        November = 11,
                        December = 12,
                        BusinessUnitCode = "A",
                        GrandTotal = 12345,
                        LicenseCurrencyCode = "PHP",
                        LicenseSubscriptionTypeCode = "A"
                    },
                    new Models.ChartModels.YearlySalesReportModel {
                        January = 11,
                        February = 12,
                        March = 13,
                        April = 14,
                        May = 15,
                        June = 16,
                        July = 17,
                        August =18,
                        September = 19,
                        October = 20,
                        November = 21,
                        December = 22,
                        BusinessUnitCode = "B",
                        GrandTotal = 346456,
                        LicenseCurrencyCode = "USD",
                        LicenseSubscriptionTypeCode = "B"
                    },
                    new Models.ChartModels.YearlySalesReportModel {
                        January = 4,
                        February = 7,
                        March = 43,
                        April = 2,
                        May = 7,
                        June = 3,
                        July = 44,
                        August =77,
                        September = 11,
                        October = 78,
                        November = 4,
                        December = 90,
                        BusinessUnitCode = "C",
                        GrandTotal = 567588,
                        LicenseCurrencyCode = "USD",
                        LicenseSubscriptionTypeCode = "C"
                    },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Models.DemoSource>> GetListForDashboardAsync(DateTime durationFrom, DateTime durationTo)
        {
            string username = _username;
            try
            {
                await Task.Delay(0);
                return new List<Models.DemoSource>
                {
                    new Models.DemoSource {
                        BillingAddressedTo = "A",
                        BillingRequestHeader_WorkflowAction = 1,
                        BRReferenceNo = "10001",
                        BusinessUnitCode = "B",
                        BusinessUnitName = "C",
                        Created = DateTime.Now,
                        CreatedBy = "D",
                        Date = DateTime.Now,
                        DurationFrom = DateTime.Now,
                        DurationTo = DateTime.Now,
                        FromName = "E",
                        Id = 1,
                        IsLocked = true,
                        LockDatetime = DateTime.Now,
                        Modified = DateTime.Now,
                        ModifiedBy = "F",
                        Note = "G",
                        Remarks = "H",
                        Thru = "I",
                        ToName = "J",
                        UniqueKey = Guid.NewGuid(),
                        UploadBatchNumber = Guid.NewGuid(),                         
                        //DemoSourceDetails = ,
                    }
                };
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
        // ~Accesses() {
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
