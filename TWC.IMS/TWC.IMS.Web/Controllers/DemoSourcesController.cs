using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using TWC.IMS.Models;
using System.Diagnostics;

namespace TWC.IMS.Web.Controllers
{
    public class DemoSourcesController : BaseController
    {
        private async Task<IEnumerable<DemoSourceViewModel>> MapToBillingRequestHeaderViewModel(IEnumerable<DemoSource> list)
        {
            List<DemoSourceViewModel> flist = new List<DemoSourceViewModel>();
            var currentAccountId = await User.Identity.GetUserIdAsync();

            foreach (var billingRequest in list)
            {
                var brheaderViewModel = new DemoSourceViewModel()
                {
                    Id = billingRequest.Id,
                    UniqueKey = billingRequest.UniqueKey,
                    BillingAddressedTo = billingRequest.BillingAddressedTo,
                    BRReferenceNo = billingRequest.BRReferenceNo,
                    BusinessUnitCode = billingRequest.BusinessUnitCode,
                    BusinessUnitName = billingRequest.BusinessUnitName,
                    Date = billingRequest.Date,
                    DurationFrom = billingRequest.DurationFrom,
                    DurationTo = billingRequest.DurationTo,
                    IsLocked = billingRequest.IsLocked,
                    LockDatetime = billingRequest.LockDatetime,
                    FromName = billingRequest.FromName,
                    ToName = billingRequest.ToName,
                    Note = billingRequest.Note,
                    Thru = billingRequest.Thru,
                    UploadBatchNumber = billingRequest.UploadBatchNumber,
                    Created = billingRequest.Created.HasValue ? billingRequest.Created.Value.DateTime.AsNullable() : null,
                    CreatedBy = billingRequest.CreatedBy != null ? billingRequest.CreatedBy : string.Empty,
                    RowVersion = billingRequest.RowVersion,
                    BillingRequestHeader_WorkflowAction = billingRequest.BillingRequestHeader_WorkflowAction,
                    Modified = billingRequest.Modified.Value.DateTime.AsNullable(),
                    ModifiedBy = billingRequest.ModifiedBy != null ? billingRequest.ModifiedBy : string.Empty,
                };

                brheaderViewModel.IsCurrentApprover = true;

                var status = "Approved";
                var remarks = "test remarks";

                brheaderViewModel.IsCreator = string.Compare(brheaderViewModel.CreatedBy, User.Identity.Name, true) == 0;
                brheaderViewModel.CurrentStatus = status;
                brheaderViewModel.Remarks = remarks;

                flist.Add(brheaderViewModel);
            }
            return flist.OrderByDescending(a => a.Modified ?? a.Created);
        }

        [SkipLogActionFilter]
        public async Task<JsonResult> GetSubscriptionTypeList(string text)
        {
            await Task.Delay(0);
            var list = new List<dynamic> {
                new { Id = 1, Code = "License" },
                new { Id = 2, Code = "Maintenance" },
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //[SkipLogActionFilter]
        //[HttpGet]
        //public async Task<JsonResult> GetCounterValuesAsync()
        //{
        //    try
        //    {
        //        await Task.Delay(0);
        //        var workflowActions = new Dictionary<string, WorkflowAction>();
        //        workflowActions.Add("A", new WorkflowAction { Id = 1, Code = "APRV", Name = "Approved", IsActive = true });
        //        workflowActions.Add("B", new WorkflowAction { Id = 2, Code = "FAPRV", Name = "For Approval", IsActive = true });
        //        var values = workflowActions.ToDictionary(
        //            x => x.Key,
        //            x => new
        //            {
        //                Id = x.Value.Id,
        //                Code = x.Value.Code,
        //                Name = x.Value.Name,
        //                IsActive = x.Value.IsActive
        //            });
        //        return Json(new { list = values }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        throw;
        //    }
        //}

        [SkipLogActionFilter]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<JsonResult> ReadDemoSourceData([DataSourceRequest] DataSourceRequest request, string durationFrom, string durationTo)
        {
            try
            {
                DateTime start;
                DateTime end;

                if (!string.IsNullOrEmpty(durationFrom))
                {
                    var isValid = DateTime.TryParseExact(durationFrom, "dd-MMM-yyyy", null, System.Globalization.DateTimeStyles.None, out start);
                    start = start.Day > 1 ? start.AddDays(-(start.Day - 1)) : start;
                    if (!isValid)
                        throw new ArgumentException("From filter is invalid.");
                }
                else
                    start = DateTime.MinValue;

                if (!string.IsNullOrEmpty(durationTo))
                {
                    var isValid = DateTime.TryParseExact(durationTo, "dd-MMM-yyyy", null, System.Globalization.DateTimeStyles.None, out end);
                    end = end.Day > 1 ? end.AddDays(-(end.Day - 1)) : end;
                    if (!isValid)
                        throw new ArgumentException("To filter is invalid.");
                }
                else
                    end = DateTime.MaxValue;

                IEnumerable<DemoSourceViewModel> list = new List<DemoSourceViewModel>();
                using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
                {
                    var data = await _demoSourcesBL.GetListForDashboardAsync(start, end).ConfigureAwait(false);
                    if (data != null)
                    {
                        list = await MapToBillingRequestHeaderViewModel(data);
                    }
                }
                return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new DataSourceResult() { Errors = new { Message = "Failed to get billing requests. ", ExceptionMessage = ex.Message } });
            }
        }

        [SkipLogActionFilter]
        public async Task<JsonResult> GetBillingRequestsByStatus([DataSourceRequest] DataSourceRequest request, string c)
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month - 1; //previous month
                var list = await _demoSourcesBL.GetBillingProgressDetailsDataAsync(year, month, c).ConfigureAwait(false);
                return Json(await list.ToDataSourceResultAsync(request).ConfigureAwait(false), JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        public async Task<JsonResult> GetTotalSales()
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month - 1;
                var list = await _demoSourcesBL.DashboardTotalSalesReportAsync(year, month).ConfigureAwait(false);
                return Json(new { value = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        [HttpGet]
        public async Task<JsonResult> GetTotalAverageRevenuePerUnit()
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month - 1;
                var list = await _demoSourcesBL.DashboardAverageRevenuePerUnitReportAsync(year, month).ConfigureAwait(false);
                return Json(new { value = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        public async Task<JsonResult> GetBillingProgress()
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month - 1;
                var list = await _demoSourcesBL.DashboardBillingProgressAsync(year, month).ConfigureAwait(false);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<JsonResult> GetCurrentMonthTop5LicenseSubscriptions()
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                var date = DateTime.Now.AddMonths(-1);
                var list = await _demoSourcesBL.GetCurrentMonthTop5LicenseSubscriptionsAsync(date).ConfigureAwait(false);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<JsonResult> GetCurrentMonthTop5BUSubscriptions()
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                var date = DateTime.Now.AddMonths(-1);
                var list = await _demoSourcesBL.GetCurrentMonthTop5BUSubscriptionsAsync(date).ConfigureAwait(false);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<JsonResult> GetCurrentMonthTop5LicenseQuantitiesPerBU([DataSourceRequest] DataSourceRequest request)
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                var date = DateTime.Now.AddMonths(-1);
                var list = await _demoSourcesBL.GetCurrentMonthLicenseQuantitiesPerBUAsync(date).ConfigureAwait(false);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<JsonResult> GetTopNSumofLicensesQuantities()
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                // one whole year
                int year = DateTime.Now.Year;
                var list = await _demoSourcesBL.GetTopNSumofLicensesQuantitiesAsync(year, isTop5: true).ConfigureAwait(false);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
    }
}