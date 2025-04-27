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
    public class AuditLogsController : BaseController
    {
        #region PRIVATE MEMBERS
        private async Task<IEnumerable<AuditLogsViewModel>> GetAuditLogsAsync(DateTime date)
        {
            var list = new List<AuditLogsViewModel>();
            using (_auditLogsBL = new AuditLogs(User.Identity.Name))
            {
                var tmpList = await _auditLogsBL.GetListAsync(date).ConfigureAwait(false);
                var finalList = tmpList.OrderByDescending(a => a.Created);
                foreach (var item in finalList)
                {
                    var obj = new AuditLogsViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.ColumnName = item.ColumnName;
                    obj.CreatedBy = item.CreatedBy;
                    obj.EventType = item.EventType;
                    obj.Id = item.Id;
                    obj.UniqueKey = item.UniqueKey;
                    obj.NewValue = item.NewValue;
                    obj.OldValue = item.OldValue;
                    obj.RowID = item.RowID;
                    obj.TableName = item.TableName;

                    list.Add(obj);
                }
            }
            return list;
        }


        #endregion

        // GET: AuditLogs
        [CustomAuthorize(AccessName = "AuditLogs.CanView")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: 
        public async Task<ActionResult> Details(string key, bool isPartial)
        {
            using (_auditLogsBL = new AuditLogs(User.Identity.Name))
            {
                var model = new AuditLogsViewModel();
                // edit mode
                if (!string.IsNullOrWhiteSpace(key))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        var obj = await _auditLogsBL.GetAsync(uniqueKey).ConfigureAwait(false);
                        if (obj != null)
                        {
                            model.Id = obj.Id;
                            model.UniqueKey = obj.UniqueKey;
                            model.ColumnName = obj.ColumnName;
                            model.EventType = obj.EventType;
                            model.NewValue = obj.NewValue;
                            model.OldValue = obj.OldValue;
                            model.RowID = obj.RowID;
                            model.TableName = obj.TableName;
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
        public async Task<ActionResult> ReadAuditLogs([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime dDate;
            if (!DateTime.TryParse(date, out dDate))
                dDate = DateTime.Now;

            var list = await GetAuditLogsAsync(dDate).ConfigureAwait(false);
            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_auditLogsBL != null)
                    _auditLogsBL = null;
            }

            base.Dispose(disposing);
        }
    }
}