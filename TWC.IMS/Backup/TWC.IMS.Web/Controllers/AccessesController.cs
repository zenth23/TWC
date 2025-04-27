using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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
    public class AccessesController : BaseController
    {
        #region PRIVATE MEMBERS

        private async Task<IEnumerable<AccessViewModel>> MapToViewModelAsync()
        {
            var list = new List<AccessViewModel>();
            using (_accessesBL = new Accesses(User.Identity.Name))
            {
                var tmpList = await _accessesBL.GetListAsync().ConfigureAwait(false);
                var finalList = tmpList.OrderBy(a => a.Name);
                foreach (var item in finalList)
                {
                    var obj = new AccessViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.CreatedBy = item.CreatedBy;
                    obj.Modified = item.Modified?.DateTime.AsNullable();
                    obj.ModifiedBy = item.ModifiedBy;
                    obj.Id = item.Id;
                    obj.UniqueKey = item.UniqueKey;
                    obj.Description = item.Description;
                    obj.Name = item.Name;

                    list.Add(obj);
                }
            }
            return list;
        }

        #endregion

        [CustomAuthorize(AccessName = "Accesses.CanView")]
        // GET: Accesses
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Details(string key)
        {
            using (_accessesBL = new Accesses(User.Identity.Name))
            {
                var model = new AccessViewModel();
                // edit mode
                if (!string.IsNullOrWhiteSpace(key))
                {
                    bool isAuthorized = await CanEditAsync("Accesses").ConfigureAwait(false);
                    if (!isAuthorized)
                        throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);

                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        var obj = await _accessesBL.GetAsync(uniqueKey).ConfigureAwait(false);
                        if (obj != null)
                        {
                            model.Id = obj.Id;
                            model.Name = obj.Name;
                            model.Description = obj.Description;
                            model.AccessRowVersion = obj.RowVersion;
                            // get audit trail from the parent module                                 
                            model.CreatedBy = string.Format("{0} {1}", obj.CreatedBy, (obj.Created == null ? "" : "on " + obj.Created.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                            model.ModifiedBy = string.Format("{0} {1}", obj.ModifiedBy, (obj.Modified == null ? "" : "on " + obj.Modified.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                        }
                        else
                            throw new HttpException(404, $"Key '{key}' not found.");
                    }
                    else
                        throw new HttpException(404, $"Invalid key.");
                }
                else
                {
                    bool isAuthorized = await CanAddAsync("Accesses").ConfigureAwait(false);
                    if (!isAuthorized)
                        throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);
                }
                
                ViewBag.Mode = string.IsNullOrWhiteSpace(key) ? "CREATE" : "EDIT";
                return View(model);
            }
        }

        [CustomAuthorize(AccessName = "Accesses.CanAdd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "CREATE")]
        public async Task<ActionResult> Create(AccessViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (_accessesBL = new Accesses(User.Identity.Name))
                    {
                        var accessObj = await _accessesBL.GetAsync(model.Name).ConfigureAwait(false);
                        if (accessObj == null)
                        {
                            accessObj = new Access();
                            accessObj.Name = model.Name.Trim();
                            accessObj.Description = model.Description;

                            await _accessesBL.InsertAsync(accessObj).ConfigureAwait(false);

                            return RedirectToAction("Index");
                        }
                        else
                            ModelState.AddModelError("", $"'{model.Name}' name already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            // force mode set to CREATE
            ViewBag.Mode = "CREATE";
            return View("details", model);
        }

        [CustomAuthorize(AccessName = "Accesses.CanEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "EDIT")]
        public async Task<ActionResult> Edit(AccessViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (_accessesBL = new Accesses(User.Identity.Name))
                    {
                        var list = await _accessesBL.GetListAsync().ConfigureAwait(false);
                        var obj = list.Where(a => string.Compare(a.Name.Trim(), model.Name.Trim(), true) == 0 && a.Id != model.Id).FirstOrDefault();
                        if (obj == null)
                        {
                            var objItem = new Access();
                            objItem.Id = model.Id;
                            objItem.Name = model.Name;
                            objItem.Description = model.Description;
                            objItem.RowVersion = model.AccessRowVersion;

                            await _accessesBL.UpdateAsync(objItem).ConfigureAwait(false);
                            return RedirectToAction("Index");
                        }
                        else
                            ModelState.AddModelError("", $"'{model.Name}' name already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            // force mode set to EDIT
            ViewBag.Mode = "EDIT";
            return View("details", model);
        }

        [CustomAuthorize(AccessName = "Accesses.CanDelete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string key)
        {
            try
            {
                using (_accessesBL = new Accesses(User.Identity.Name))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        await _accessesBL.DeleteAsync(uniqueKey).ConfigureAwait(false);
                        return Json(new { Status = "SUCCESS", Message = "Record successfully deleted." });
                    }
                    else
                        return Json(new { Status = "ERROR", Message = "Invalid key." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERROR", Message = ex.Message });
            }
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> ReadAccesses([DataSourceRequest]DataSourceRequest request)
        {
            var list = await MapToViewModelAsync();
            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}