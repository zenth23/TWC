using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TWC.IMS.BL;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class ErrorLogsController : BaseController
    {
        #region PRIVATE MEMBERS

        private async Task<IEnumerable<ErrorLogsViewModel>> GetErrorLogsAsync(DateTime date)
        {
            var list = new List<ErrorLogsViewModel>();
            using (_errorLogsBL = new ErrorLogs(User.Identity.Name))
            {
                var tmpList = await _errorLogsBL.GetListAsync(date).ConfigureAwait(false);
                var finalList = tmpList.OrderByDescending(a => a.Created);
                foreach (var item in finalList)
                {
                    var obj = new ErrorLogsViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.CreatedBy = item.CreatedBy;
                    obj.Id = item.Id;
                    obj.ErrorMessage = item.ErrorMessage;
                    obj.ErrorNumber = item.ErrorNumber;
                    obj.Exception = item.Exception;
                    obj.FriendlyErrorMessage = item.FriendlyErrorMessage;
                    obj.MessageType = item.MessageType;
                    obj.MethodName = item.MethodName;
                    obj.UniqueKey = item.UniqueKey;
                    obj.AppVersion = item.AppVersion;
                    obj.IsMobileDevice = item.IsMobileDevice;
                    obj.Environment = item.Environment;
                    obj.ImpactLevel = item.ImpactLevel;
                    obj.ParamData = item.ParamData;
                    obj.UserRole = item.UserRole;

                    list.Add(obj);
                }
                return list;
            }
        }


        #endregion

        [CustomAuthorize(Users = "smitsadmin.projectmold")]
        // GET: ErrorLogs
        public ActionResult Index()
        {
            return View();
        }

        // GET: 
        public async Task<ActionResult> Details(string key, bool isPartial)
        {
            using (_errorLogsBL = new ErrorLogs(User.Identity.Name))
            {
                var model = new ErrorLogsViewModel();
                // edit mode
                if (!string.IsNullOrWhiteSpace(key))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        var obj = await _errorLogsBL.GetAsync(uniqueKey).ConfigureAwait(false);
                        if (obj != null)
                        {
                            model.Id = obj.Id;
                            model.ErrorMessage = obj.ErrorMessage;
                            model.ErrorNumber = obj.ErrorNumber;
                            model.Exception = obj.Exception;
                            model.FriendlyErrorMessage = obj.FriendlyErrorMessage;
                            model.MessageType = obj.MessageType;
                            model.MethodName = obj.MethodName;
                            model.UniqueKey = obj.UniqueKey;
                            model.AppVersion = obj.AppVersion;
                            model.IsMobileDevice = obj.IsMobileDevice;
                            model.Environment = obj.Environment;
                            model.ImpactLevel = obj.ImpactLevel;
                            model.ParamData = obj.ParamData;
                            model.UserRole = obj.UserRole;
                            // get audit trail from the parent module                                 
                            model.CreatedBy = obj.CreatedBy;
                            model.Created = obj.Created.Value;
                        }
                        else
                            throw new HttpException(404, $"Key '{key}' not found.");
                    }
                    else
                        throw new HttpException(404, $"Invalid key.");
                }

                ViewData["IS_PARTIAL"] = isPartial;

                if (isPartial)
                    return PartialView("Details", model);

                return View(model);
            }
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> ReadErrorLogs([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime dDate;
            if (!DateTime.TryParse(date, out dDate))
                dDate = DateTime.Now;

            var list = await GetErrorLogsAsync(dDate).ConfigureAwait(false);
            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_errorLogsBL != null)
                    _errorLogsBL = null;
            }

            base.Dispose(disposing);
        }
    }
}