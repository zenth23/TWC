using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TWC.IMS.BL;
using TWC.IMS.Models;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class AccessModulesController : BaseController
    {
        #region PRIVATE MEMBERS

        #endregion

        [SkipLogActionFilter]
        public async Task<JsonResult> ReadModuleAccesses([DataSourceRequest]DataSourceRequest request)
        {
            string username = User.Identity.Name;
            using (_accessesBL = new Accesses(username))
            {
                string roleid = Request.Form["roleid"];
                var list = await _accessesBL.GetModuleAccessesDataTableAsync(username, roleid).ConfigureAwait(false);
                DataSourceResult result = list.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        public ActionResult GetModuleAccessesPartialView()
        {
            string username = User.Identity.Name;
            using (_accessesBL = new Accesses(username))
            {
                var list = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => _accessesBL.GetModuleAccessesDataTableAsync(username, null, schemaOnly: true));
                return PartialView("_AccessModuleGridEdit3", list);
            }
        }

        [SkipLogActionFilter]
        public ActionResult GetModuleAccessesByRolePartialView(string roleid)
        {
            string username = User.Identity.Name;
            using (_accessesBL = new Accesses(username))
            {
                var list = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => _accessesBL.GetModuleAccessesDataTableAsync(username, roleid, true));
                return PartialView("_AccessModuleGrid2", list);
            }
        }

        // GET: AccessModules
        [Authorize(Users = "twcusr")]
        //[CustomAuthorize(AccessName = "Accesses.CanView")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Users = "twcusr")]
        //[CustomAuthorize(AccessName = "Accesses.CanView")]
        public async Task<ActionResult> Details(string key)
        {
            using (_moduleAccessesBL = new ModuleAccesses(User.Identity.Name))
            using (_modulesBL = new Modules(User.Identity.Name))
            using (_accessesBL = new Accesses(User.Identity.Name))
            {
                var model = new AccessModuleViewModel();
                var accesses = await _accessesBL.GetListAsync().ConfigureAwait(false);
                model.Accesses = accesses.Select(a => new AccessViewModel()
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList();

                // edit mode
                if (!string.IsNullOrWhiteSpace(key))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        var module = await _modulesBL.GetAsync(uniqueKey).ConfigureAwait(false);
                        if (module != null)
                        {
                            model.Id = module.Id;
                            model.UniqueKey = module.UniqueKey;
                            model.Name = module.Name;
                            model.Description = module.Description;
                            model.URL = module.URL;
                            model.IconClassName = module.IconClassName;
                            model.AccessModuleRowVersion = module.RowVersion;

                            // get audit trail from the parent module                                 
                            model.CreatedBy = string.Format("{0} {1}", module.CreatedBy, (module.Created == null ? "" : "on " + module.Created.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                            model.ModifiedBy = string.Format("{0} {1}", module.ModifiedBy, (module.Modified == null ? "" : "on " + module.Modified.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));

                            model.Accesses = accesses.Select(a => new AccessViewModel()
                            {
                                Id = a.Id,
                                Name = a.Name,
                                IsChecked = module.ModuleAccesses.Any(b => b.ModuleAccess_Access == a.Id)
                            }).ToList();
                        }
                        else
                            throw new HttpException(404, $"Key '{key}' not found.");
                    }
                    else
                        throw new HttpException(404, $"Invalid key.");
                }

                ViewBag.Referrer = Request.UrlReferrer;
                ViewBag.Mode = string.IsNullOrWhiteSpace(key) ? "CREATE" : "EDIT";
                return View(model);
            }
        }

        [Authorize(Users = "twcusr")]
        //[CustomAuthorize(AccessName = "Accesses.CanAdd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "CREATE")]
        public async Task<ActionResult> Create(AccessModuleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (_modulesBL = new Modules(User.Identity.Name))
                    using (_moduleAccessesBL = new ModuleAccesses(User.Identity.Name))
                    {
                        var mObj = await _modulesBL.GetAsync(model.Name).ConfigureAwait(false);
                        if (mObj == null)
                        {
                            mObj = new Module();
                            mObj.Name = model.Name.Trim();
                            mObj.Description = model.Description;
                            mObj.URL = model.URL;
                            mObj.IconClassName = model.IconClassName;
                            int mid = await _modulesBL.InsertAsync(mObj).ConfigureAwait(false);

                            foreach (var item in model.Accesses)
                            {
                                if (item.IsChecked)
                                {
                                    var maObj = new ModuleAccess();
                                    maObj.ModuleAccess_Access = item.Id;
                                    maObj.ModuleAccess_Module = mid;

                                    await _moduleAccessesBL.InsertAsync(maObj).ConfigureAwait(false);
                                }
                            }
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

            ViewBag.Referrer = Request.UrlReferrer;
            // force mode set to CREATE
            ViewBag.Mode = "CREATE";
            return View("details", model);
        }

        [Authorize(Users = "twcusr")]
        //[CustomAuthorize(AccessName = "Accesses.CanEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "EDIT")]
        public async Task<ActionResult> Edit(AccessModuleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (_modulesBL = new Modules(User.Identity.Name))
                    {
                        var list = await _modulesBL.GetListAsync().ConfigureAwait(false);
                        var module = list.Where(a => string.Compare(a.Name.Trim(), model.Name.Trim(), true) == 0 && a.Id != model.Id).FirstOrDefault();
                        if (module == null)
                        {
                            using (_moduleAccessesBL = new ModuleAccesses(User.Identity.Name))
                            {
                                var maList = await _moduleAccessesBL.GetListAsync(model.Id).ConfigureAwait(false);
                                var maCurrentList = maList.Select(a => new
                                {
                                    ModuleAccess_Module = a.ModuleAccess_Module,
                                    ModuleAccess_Access = a.ModuleAccess_Access
                                })
                                .OrderBy(a => a.ModuleAccess_Access);

                                var maNewList = model.Accesses.Where(a => a.IsChecked)
                                .Select(a => new
                                {
                                    ModuleAccess_Module = model.Id,
                                    ModuleAccess_Access = a.Id
                                })
                                .OrderBy(a => a.ModuleAccess_Access);

                                var removedAccesses = maCurrentList.Except(maNewList);
                                var addedAccesses = maNewList.Except(maCurrentList);
                                var changesList = removedAccesses.Concat(addedAccesses);
                                if (changesList.Any())
                                {
                                    var hasError = ModelState.Any(a => a.Value.Errors.Count > 0);
                                    if (!hasError)
                                    {
                                        foreach (var item in removedAccesses)
                                        {
                                            await _moduleAccessesBL.DeleteByModuleAsync(item.ModuleAccess_Module, item.ModuleAccess_Access).ConfigureAwait(false);
                                        }

                                        // insert new acceses
                                        foreach (var item in addedAccesses)
                                        {
                                            var maObj = new ModuleAccess();
                                            maObj.Modified = DateTime.Now;
                                            maObj.ModifiedBy = User.Identity.Name;
                                            maObj.ModuleAccess_Access = item.ModuleAccess_Access;
                                            maObj.ModuleAccess_Module = item.ModuleAccess_Module;

                                            await _moduleAccessesBL.InsertAsync(maObj).ConfigureAwait(false);
                                        }

                                        // update the Module table as well
                                        var moduleItem = new Module();
                                        moduleItem.Id = model.Id;
                                        moduleItem.Name = model.Name.Trim();
                                        moduleItem.Description = model.Description;
                                        moduleItem.RowVersion = model.AccessModuleRowVersion;
                                        moduleItem.IconClassName = model.IconClassName;
                                        moduleItem.URL = model.URL;

                                        await _modulesBL.UpdateAsync(moduleItem).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        ViewBag.Referrer = Request.UrlReferrer;
                                        // force mode set to EDIT
                                        ViewBag.Mode = "EDIT";
                                        return View("details", model);
                                    }
                                }

                                return RedirectToAction("Index");
                            }
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

            ViewBag.Referrer = Request.UrlReferrer;
            // force mode set to EDIT
            ViewBag.Mode = "EDIT";
            return View("details", model);
        }

        [Authorize(Users = "twcusr")]
        //[CustomAuthorize(AccessName = "Accesses.CanDelete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string key)
        {
            try
            {
                using (_modulesBL = new Modules(User.Identity.Name))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        await _modulesBL.DeleteAsync(uniqueKey).ConfigureAwait(false);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_moduleAccessesBL != null)
                    _moduleAccessesBL = null;

                if (_modulesBL != null)
                    _modulesBL = null;

                if (_accessesBL != null)
                    _accessesBL = null;
            }

            base.Dispose(disposing);
        }
    }
}
