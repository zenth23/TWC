using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    public class AdminDashboardChartsController : BaseController
    {
        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetErrorFrequencyChartData(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetErrorFrequencyDataAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetErrorRateChartData(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetErrorRateChartDataAsync(date.Year).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetErrorRateChartDataCvp(string d)
        {
            DateTime currDate = DateTime.Now;
            DateTime.TryParse(d, out currDate);
            DateTime prevDate = currDate.AddMonths(-1);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetErrorRateChartDataCvpAsync(currDate, prevDate).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetErrorTrendChartData(string d)
        {
            DateTime date = DateTime.Now;
            int dayToday = date.Day;
            DateTime.TryParse(d, out date);
            date = new DateTime(date.Year, date.Month, dayToday);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetErrorTrendChartDataAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetErrorSeverityChartData(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetErrorSeverityChartDataAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetApplicationVersionErrorDistributionChartData(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetApplicationVersionErrorDistributionChartDataAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public JsonResult GetErrorImpactChartData(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            return Json(null);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetUserRelatedMetricsChartData([DataSourceRequest]DataSourceRequest request, string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetUserRelatedMetricsChartDataAsync(date).ConfigureAwait(false);
            DataSourceResult result = data.ToDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetLogCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetLogCountersAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetPageHitsCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _userActivityLogsBL = new BL.UserActivityLogs(User.Identity.Name);
            var data = await _userActivityLogsBL.GetHitCountListAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetMethodHitsCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetMethodHitCountListAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetTableHitsCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _auditLogsBL = new BL.AuditLogs(User.Identity.Name);
            var data = await _auditLogsBL.GetTableHitCountListAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetRecipientHitsCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _emailLogsBL = new BL.EmailLogs(User.Identity.Name);
            var data = await _emailLogsBL.GetRecipientHitCountListAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetUserActivityLogCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _userActivityLogsBL = new BL.UserActivityLogs(User.Identity.Name);
            var data = await _userActivityLogsBL.GetLogCountersAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetErrorLogCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _errorLogsBL = new BL.ErrorLogs(User.Identity.Name);
            var data = await _errorLogsBL.GetLogCountersAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetAuditLogCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _auditLogsBL = new BL.AuditLogs(User.Identity.Name);
            var data = await _auditLogsBL.GetLogCountersAsync(date).ConfigureAwait(false);
            return Json(data);
        }

        [HttpPost]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Charts")]
        public async Task<JsonResult> GetEmailLogCounters(string d)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(d, out date);
            _emailLogsBL = new BL.EmailLogs(User.Identity.Name);
            var data = await _emailLogsBL.GetLogCountersAsync(date).ConfigureAwait(false);
            return Json(data);
        }
    }
}