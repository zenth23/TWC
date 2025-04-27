using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class ReportGeneratorController : BaseController
    {
        private string _standardReportsPath = "~\\Reports\\StandardReports";

        // GET: ReportGenerator
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(AccessName = "Reports.CanView")]
        public ActionResult StandardReports()
        {
            return View();
        }

        #region PDF
        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateRoleMasterdataReportPdf()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateRoleMasterdataReportAsync(ReportFileType.PDF, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateUserMasterdataReportPdf()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateUserMasterdataReportAsync(ReportFileType.PDF, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateSystemModulesMasterdataReportPdf()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateSystemModulesMasterdataReportAsync(ReportFileType.PDF, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateAccessControlListReportPdf()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateAccessControlListReportAsync(ReportFileType.PDF, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateUserAccessMatrixReportPdf()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateUserAccessMatrixReportAsync(ReportFileType.PDF, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, filename);
            }
        }
        #endregion

        #region EXCEL
        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateRoleMasterdataReportExcel()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateRoleMasterdataReportAsync(ReportFileType.EXCELOPENXML, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateUserMasterdataReportExcel()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateUserMasterdataReportAsync(ReportFileType.EXCELOPENXML, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateSystemModulesMasterdataReportExcel()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateSystemModulesMasterdataReportAsync(ReportFileType.EXCELOPENXML, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateAccessControlListReportExcel()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateAccessControlListReportAsync(ReportFileType.EXCELOPENXML, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }

        [OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<FileResult> GenerateUserAccessMatrixReportExcel()
        {
            using (_reportsBL = new BL.Reports(this.HttpContext.User.Identity.Name))
            {
                string reportPath = Server.MapPath(_standardReportsPath);
                var result = await _reportsBL.GenerateUserAccessMatrixReportAsync(ReportFileType.EXCELOPENXML, reportPath).ConfigureAwait(false);
                string filename = Path.GetFileName(result.Item1);
                byte[] fileBytes = result.Item2;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
        }
        #endregion

        //[CustomAuthorize(AccessName = "Reports.CanView")]
        public async Task<ActionResult> YearlySalesReport()
        {
            await ReportHelpers.GetReportExpirationAsync(this.HttpContext).ConfigureAwait(false);
            return View();
        }

        [SkipLogActionFilter]
        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<ActionResult> ReadDashboardYearlyReportsUnits([DataSourceRequest]DataSourceRequest request, string subscriptionTypeCode)
        {
            using (_demoSourcesBL = new BL.DemoSources(User.Identity.Name))
            {
                int year = DateTime.Now.Year;
                var list = await _demoSourcesBL.DashboardYearlyReportAsync(subscriptionTypeCode, year).ConfigureAwait(false);
                DataSourceResult result = list.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> GetLatestReport(string fileName)
        {
            string username = User.Identity.Name;
            using (_reportCachesBL = new BL.ReportCaches(username))
            {
                try
                {
                    var obj = await _reportCachesBL.GetLastAsync(fileName).ConfigureAwait(false);
                    if (obj != null)
                    {
                        return Json(new { status = "SUCCESS", data = obj, currentDate = DateTime.Now }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = "FAILED" });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public async Task<ActionResult> CacheReport(string fileName, string contentType, string base64)
        {
            string username = User.Identity.Name;
            using (_reportCachesBL = new BL.ReportCaches(username))
            {
                try
                {
                    var obj = await _reportCachesBL.GetLastAsync(fileName).ConfigureAwait(false);
                    if (obj == null || obj.ExpirationDate < DateTime.Now || obj.ReportName != fileName.ToLower())
                    {
                        DateTime result = await _reportCachesBL.DeleteInsertAsync(fileName, contentType, base64).ConfigureAwait(false);
                        var fileContents = Convert.FromBase64String(base64);
                        return File(fileContents, contentType, fileName);
                    }
                    else
                    {
                        var fileContents = Convert.FromBase64String(base64);
                        return File(fileContents, contentType, fileName);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        //[OutputCache(CacheProfile = "StandardCache_0060_Server_Reports")]
        public async Task<ActionResult> DownloadReport(string report)
        {
            string username = User.Identity.Name;
            using (_reportCachesBL = new BL.ReportCaches(username))
            {
                var file = await _reportCachesBL.GetLastAsync(report).ConfigureAwait(false);
                if (file != null && file.ExpirationDate >= DateTime.Now)
                {
                    string base64 = file.ReportImage;
                    string contentType = file.ContentType;
                    string fileName = file.ReportName;
                    var fileContents = Convert.FromBase64String(base64);
                    return File(fileContents, contentType, fileName);
                }
                return RedirectToAction("index", "home");
            }
        }
    }
}