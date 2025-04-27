using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TWC.IMS.Common;
using TWC.IMS.BL;
using TWC.IMS.Models;
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
    public class UserActivityLogsController : BaseController
    {
        #region PRIVATE MEMBERS
        private async Task<IEnumerable<UserActivityLogsViewModel>> GetUserActivityLogsAsync(DateTime date)
        {
            var list = new List<UserActivityLogsViewModel>();
            using (_userActivityLogsBL = new UserActivityLogs(User.Identity.Name))
            {
                var tmpList = await _userActivityLogsBL.GetListAsync(date).ConfigureAwait(false);
                var finalList = tmpList.OrderByDescending(a => a.Created);
                foreach (var item in finalList)
                {
                    var obj = new UserActivityLogsViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.CreatedBy = item.CreatedBy;
                    obj.Id = item.Id;
                    obj.UniqueKey = item.UniqueKey;
                    obj.AbsoluteUrl = item.AbsoluteUrl;
                    obj.Activity = item.Activity;
                    obj.MethodType = item.MethodType;
                    obj.ClientIPAddress = item.ClientIPAddress;
                    obj.UserAgent = item.UserAgent;
                    obj.AppVersion = item.AppVersion;
                    obj.IsMobileDevice = item.IsMobileDevice;
                    obj.SessionId = item.SessionId;
                    obj.SessionStart = item.SessionStart;
                    obj.SessionTimeout = item.SessionTimeout;
                    obj.UserRole = item.UserRole;
                    obj.FormData = item.FormData;

                    list.Add(obj);
                }
            }
            return list;
        }

        private async Task<DataSourceResult> GetUserActivityLogsAsync(DateTime date, DataSourceRequest request)
        {
            var list = new List<UserActivityLogsViewModel>();
            using (_userActivityLogsBL = new UserActivityLogs(User.Identity.Name))
            {
                var tmpList = await _userActivityLogsBL.GetListAsync(date, request).ConfigureAwait(false);
                var castList = tmpList.Data.Cast<TWC.IMS.Models.UserActivityLog>();
                foreach (var item in castList)
                {
                    var obj = new UserActivityLogsViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.CreatedBy = item.CreatedBy;
                    obj.Id = item.Id;
                    obj.UniqueKey = item.UniqueKey;
                    obj.AbsoluteUrl = item.AbsoluteUrl;
                    obj.Activity = item.Activity;
                    obj.MethodType = item.MethodType;
                    obj.ClientIPAddress = item.ClientIPAddress;
                    obj.UserAgent = item.UserAgent;
                    obj.AppVersion = item.AppVersion;
                    obj.IsMobileDevice = item.IsMobileDevice;
                    obj.SessionId = item.SessionId;
                    obj.SessionStart = item.SessionStart;
                    obj.SessionTimeout = item.SessionTimeout;
                    obj.UserRole = item.UserRole;
                    obj.FormData = item.FormData;

                    list.Add(obj);
                }
                tmpList.Data = list;
                return tmpList;
            }
        }

        private IEnumerable<RecentActivitiesViewModel> MapToRecentActivitiesViewModel(IEnumerable<UserActivityLog> list)
        {
            List<RecentActivitiesViewModel> raList = new List<RecentActivitiesViewModel>();

            foreach (var obj in list)
            {
                DateTime curr = obj.Created.Value.DateTime;
                var currTime = curr.ToTimeAgo();
                var ra = new RecentActivitiesViewModel()
                {
                    Activity = obj.Activity,
                    Time = currTime
                };

                raList.Add(ra);
            }
            return raList;
        }
        #endregion

        // GET: UserActivityLogs
        [CustomAuthorize(AccessName = "UserActivityLogs.CanView")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: 
        public async Task<ActionResult> Details(string key, bool isPartial)
        {
            using (_userActivityLogsBL = new UserActivityLogs(User.Identity.Name))
            {
                var model = new UserActivityLogsViewModel();
                // edit mode
                if (!string.IsNullOrWhiteSpace(key))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        var obj = await _userActivityLogsBL.GetAsync(uniqueKey).ConfigureAwait(false);
                        if (obj != null)
                        {
                            model.Id = obj.Id;
                            model.UniqueKey = obj.UniqueKey;
                            model.AppVersion = obj.AppVersion;
                            model.IsMobileDevice = obj.IsMobileDevice;
                            model.AbsoluteUrl = obj.AbsoluteUrl;
                            model.Activity = obj.Activity;
                            model.ClientIPAddress = obj.ClientIPAddress;
                            model.MethodType = obj.MethodType;
                            model.SessionId = obj.SessionId;
                            model.SessionStart = obj.SessionStart;
                            model.SessionTimeout = obj.SessionTimeout;
                            model.UserAgent = obj.UserAgent;
                            model.UserRole = obj.UserRole;
                            model.FormData = obj.FormData;
                            // get audit trail from the parent module                                 
                            model.CreatedBy = obj.CreatedBy;
                            model.Created = obj.Created;
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
        public async Task<ActionResult> ReadUserActivityLogs([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime dDate;
            if (!DateTime.TryParse(date, out dDate))
                dDate = DateTime.Now;

            if (!request.Sorts.Any())
                request.Sorts = new[] { new SortDescriptor("Created", System.ComponentModel.ListSortDirection.Descending) };

            var list = await GetUserActivityLogsAsync(dDate, request).ConfigureAwait(false);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [SkipLogActionFilter]
        public PartialViewResult GetRecentActivities()
        {
            var username = User.Identity.Name;
            using (_userActivityLogsBL = new BL.UserActivityLogs(username))
            {
                var recentActivities = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => _userActivityLogsBL.GetListRecentActivitiesAsync(username));
                return PartialView("_RecentActivities", MapToRecentActivitiesViewModel(recentActivities));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userActivityLogsBL != null)
                    _userActivityLogsBL = null;
            }

            base.Dispose(disposing);
        }
    }
}