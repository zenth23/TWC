using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using TWC.IMS.BL;
using TWC.IMS.Models;
using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class SystemConfigController : BaseController
    {
        #region PRIVATE MEMBERS
        private async Task<IEnumerable<SystemConfigsViewModel>> MapToViewModelAsync()
        {
            var list = new List<SystemConfigsViewModel>();
            using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
            {
                var tmpList = await _systemConfigsBL.GetListAsync().ConfigureAwait(false);
                var finalList = tmpList.OrderBy(a => a.Name);
                foreach (var item in finalList)
                {
                    var obj = new SystemConfigsViewModel();
                    obj.Created = item.Created?.DateTime.AsNullable();
                    obj.CreatedBy = item.CreatedBy;
                    obj.Modified = item.Modified?.DateTime.AsNullable();
                    obj.ModifiedBy = item.ModifiedBy;
                    obj.Id = item.Id;
                    obj.UniqueKey = item.UniqueKey;
                    obj.Description = item.Description;
                    obj.Name = item.Name;
                    obj.Value = item.Value;

                    list.Add(obj);
                }
            }
            return list;
        }

        #endregion

        // GET: SystemConfig
        [CustomAuthorize(AccessName = "SystemConfig.CanView")]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Details(string key)
        {
            using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
            {
                bool isHtml = false;
                var model = new SystemConfigsViewModel();
                // edit mode
                if (!string.IsNullOrWhiteSpace(key))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        var obj = await _systemConfigsBL.GetAsync(uniqueKey).ConfigureAwait(false);
                        if (obj != null)
                        {
                            model.Id = obj.Id;
                            model.UniqueKey = obj.UniqueKey;
                            model.Name = obj.Name;
                            model.Value = obj.Value;
                            model.Description = obj.Description;
                            model.ConfigRowVersion = obj.RowVersion;
                            // get audit trail from the parent module                                 
                            model.CreatedBy = string.Format("{0} {1}", obj.CreatedBy, (obj.Created == null ? "" : "on " + obj.Created.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                            model.ModifiedBy = string.Format("{0} {1}", obj.ModifiedBy, (obj.Modified == null ? "" : "on " + obj.Modified.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));

                            // valid if value is HTML
                            var regEx = new Regex(@"<(\s*[(\/?)\w+]*)");
                            isHtml = regEx.IsMatch(model.Value ?? "");
                        }
                        else
                            throw new HttpException(404, $"Key '{key}' not found.");
                    }
                    else
                        throw new HttpException(404, $"Invalid key.");
                }

                ViewBag.IsValueHtml = isHtml;
                ViewBag.Mode = string.IsNullOrWhiteSpace(key) ? "CREATE" : "EDIT";
                return View(model);
            }
        }

        [CustomAuthorize(AccessName = "SystemConfig.CanAdd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "CREATE")]
        public async Task<ActionResult> Create(SystemConfigsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
                    {
                        var scObj = await _systemConfigsBL.GetAsync(model.Name).ConfigureAwait(false);
                        if (scObj == null)
                        {
                            scObj = new SystemConfig();
                            scObj.Name = model.Name.Trim();
                            scObj.Description = model.Description;
                            scObj.Value = model.Value;
                            await _systemConfigsBL.InsertAsync(scObj, model.EncryptValue).ConfigureAwait(false);

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

        [CustomAuthorize(AccessName = "SystemConfig.CanEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "EDIT")]
        public async Task<ActionResult> Edit(SystemConfigsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
                    {
                        var list = await _systemConfigsBL.GetListAsync().ConfigureAwait(false);
                        var obj = list.Where(a => string.Compare(a.Name.Trim(), model.Name.Trim(), true) == 0 && a.Id != model.Id).FirstOrDefault();
                        if (obj == null)
                        {
                            var objItem = new SystemConfig();
                            objItem.Id = model.Id;
                            objItem.UniqueKey = model.UniqueKey;
                            objItem.Name = model.Name;
                            objItem.Value = model.Value;
                            objItem.Description = model.Description;
                            objItem.RowVersion = model.ConfigRowVersion;

                            await _systemConfigsBL.UpdateAsync(objItem, model.EncryptValue).ConfigureAwait(false);
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

        [CustomAuthorize(AccessName = "SystemConfig.CanDelete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string key)
        {
            try
            {
                using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
                {
                    Guid uniqueKey = Guid.Empty;
                    bool isValid = Guid.TryParse(key, out uniqueKey);
                    if (isValid)
                    {
                        await _systemConfigsBL.DeleteAsync(uniqueKey).ConfigureAwait(false);
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

        [CustomAuthorize(AccessName = "SystemConfig.CanView")]
        public async Task<ActionResult> StatusObjectsConfig()
        {
            var isAdmin = await IsCurrentUserAdminAsync().ConfigureAwait(false);
            if (isAdmin)
            {
                using (_statusBL = new BL.StatusSets(User.Identity.Name))
                {
                    var statusList = await _statusBL.GetListAsync().ConfigureAwait(false);
                    ViewData["StatusList"] = statusList.OrderBy(a => a.Name).Select(a => new StatusViewModel
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList();

                    return View();
                }
            }
            else
                throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NONADMIN_ROLE);
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> ReadSystemConfigs([DataSourceRequest]DataSourceRequest request)
        {
            var list = await MapToViewModelAsync();
            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> ReadRequiredConfigs([DataSourceRequest]DataSourceRequest request)
        {
            using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
            {
                var list = await _systemConfigsBL.GetRequiredConfigsListAsync().ConfigureAwait(false);
                DataSourceResult result = list.ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [SkipLogActionFilter]
        public async Task<ActionResult> ReadStatusObjectsConfig([DataSourceRequest] DataSourceRequest request)
        {
            using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
            {
                var scVal = await _systemConfigsBL.GetValueAsync(SystemConfigName.STATUS_OBJECTS_LIST).ConfigureAwait(false);
                var configs = scVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var list = await _systemConfigsBL.GetListAsync(configs).ConfigureAwait(false);

                var flist = new List<ConfigObjectViewModel>();
                foreach (var item in list)
                {
                    try
                    {
                        var statusObj = JsonConvert.DeserializeObject<StatusSet>(item.Value);
                        if (statusObj != null)
                        {
                            var obj = new ConfigObjectViewModel
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Value = item.Value,
                                Status = new List<StatusViewModel> {
                                    new StatusViewModel {
                                        Id = statusObj.Id,
                                        Name = statusObj.Name
                                    }
                                }
                            };
                            flist.Add(obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        // unsupported objects go here
                        if (ex.Message.Contains("array"))
                        {
                            var statusObjs = JsonConvert.DeserializeObject<IEnumerable<StatusViewModel>>(item.Value);
                            var obj = new ConfigObjectViewModel
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Value = item.Value,
                                Status = statusObjs
                            };
                            flist.Add(obj);
                        }
                    }
                }

                DataSourceResult result = flist.OrderBy(a => a.Name).ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(AccessName = "SystemConfigs.CanEdit")]
        [HttpPost]
        public async Task<ActionResult> UpdateStatusObjectsConfig([DataSourceRequest] DataSourceRequest request, ConfigObjectViewModel model)
        {
            using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
            {
                if (model != null && ModelState.IsValid)
                {
                    string value = "";
                    if (model.Status.Count() > 1)
                    {
                        value = JsonConvert.SerializeObject(model.Status);
                    }
                    else
                    {
                        var obj = model.Status.FirstOrDefault();
                        value = JsonConvert.SerializeObject(obj);
                    }

                    var scObj = new SystemConfig
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Value = value
                    };
                    var result = await _systemConfigsBL.UpdateAsync(scObj).ConfigureAwait(false);
                }
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_systemConfigsBL != null)
                    _systemConfigsBL = null;
            }

            base.Dispose(disposing);
        }
    }
}