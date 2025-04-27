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
    public class EmailLogsController : BaseController
    {
        #region PRIVATE MEMBERS

        private async Task<IEnumerable<EmailLogsViewModel>> GetEmailLogsAsync(DateTime date)
        {
            var list = new List<EmailLogsViewModel>();
            using (_emailLogsBL = new EmailLogs(User.Identity.Name))
            {
                var tmpList = await _emailLogsBL.GetListAsync(date).ConfigureAwait(false);
                var finalList = tmpList.OrderByDescending(a => a.Created);
                foreach (var item in finalList)
                {
                    var obj = new EmailLogsViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.CreatedBy = item.CreatedBy;
                    obj.Id = item.Id;
                    obj.UniqueKey = item.UniqueKey;
                    obj.Bcc = item.Bcc;
                    obj.Body = item.Body;
                    obj.From = item.From;
                    obj.ResentDatetime = item.ResentDatetime;
                    obj.Status = item.Status;
                    obj.Subject = item.Subject;
                    obj.To = item.To;

                    list.Add(obj);
                }
                return list;
            }
        }


        #endregion

        [CustomAuthorize(Users = "twcusr")]
        // GET: EmailLogs
        public ActionResult Index()
        {
            return View();
        }

        // GET: 
        public async Task<ActionResult> Details(string key, bool isPartial)
        {
            using (_emailLogsBL = new EmailLogs(User.Identity.Name))
            {
                var model = new EmailLogsViewModel();
                // edit mode
                if (!string.IsNullOrWhiteSpace(key))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        var obj = await _emailLogsBL.GetAsync(uniqueKey).ConfigureAwait(false);
                        if (obj != null)
                        {
                            model.Id = obj.Id;
                            model.UniqueKey = obj.UniqueKey;
                            model.Bcc = obj.Bcc;
                            model.Body = obj.Body;
                            model.From = obj.From;
                            model.ResentDatetime = obj.ResentDatetime;
                            model.Status = obj.Status;
                            model.Subject = obj.Subject;
                            model.To = obj.To;
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
        public async Task<ActionResult> ReadEmailLogs([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime dDate;
            if (!DateTime.TryParse(date, out dDate))
                dDate = DateTime.Now;

            var list = await GetEmailLogsAsync(dDate).ConfigureAwait(false);
            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_emailLogsBL != null)
                    _emailLogsBL = null;
            }

            base.Dispose(disposing);
        }
    }
}