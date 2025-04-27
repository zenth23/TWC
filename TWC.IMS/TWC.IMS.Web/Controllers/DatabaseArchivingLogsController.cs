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
    public class DatabaseArchivingLogsController : BaseController
    {
        #region PRIVATE MEMBERS
        private async Task<IEnumerable<DatabaseArchivingLogsViewModel>> GetLogsAsync(DateTime date)
        {
            var list = new List<DatabaseArchivingLogsViewModel>();
            using (_databaseArchivingLogsBL = new DatabaseArchivingLogs(User.Identity.Name))
            {
                var tmpList = await _databaseArchivingLogsBL.GetListAsync(date).ConfigureAwait(false);
                var finalList = tmpList.OrderByDescending(a => a.Created);
                foreach (var item in finalList)
                {
                    var obj = new DatabaseArchivingLogsViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.CreatedBy = item.CreatedBy;
                    obj.Id = item.Id;
                    obj.Name = item.Name;
                    obj.Description = item.Description;

                    list.Add(obj);
                }
                return list;
            }
        }

        #endregion

        // GET: DatabaseArchivingLogs
        [CustomAuthorize(AccessName = "DatabaseArchivingLogs.CanView")]
        public ActionResult Index()
        {
            return View();
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> ReadLogs([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime dDate;
            if (!DateTime.TryParse(date, out dDate))
                dDate = DateTime.Now;

            var list = await GetLogsAsync(dDate).ConfigureAwait(false);
            DataSourceResult result = list.OrderByDescending(a => a.Created).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_databaseArchivingLogsBL != null)
                    _databaseArchivingLogsBL = null;
            }

            base.Dispose(disposing);
        }
    }
}