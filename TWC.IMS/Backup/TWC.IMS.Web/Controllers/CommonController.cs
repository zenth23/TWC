using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TWC.IMS.Common;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class CommonController : BaseController
    {
       

        [SkipLogActionFilter]
        public async Task<ActionResult> GetRoleComboBoxListAsync(string searchKey)
        {
            using (var roleDetailBL = new BL.RoleDetails(User.Identity.Name))
            {
                var list = await roleDetailBL.GetListAsync(true).ConfigureAwait(false);
                var filteredList = list.Where(x => string.IsNullOrWhiteSpace(searchKey) || x.AspNetRole.Name.IndexOf(searchKey ?? "", StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                return Json(filteredList.Select(x => new { Id = x.Id, Name = x.AspNetRole.Name }), JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> GetStatusSets(string searchKey)
        {
            using (_statusBL = new BL.StatusSets(User.Identity.Name))
            {
                var list = await _statusBL.GetListAsync().ConfigureAwait(false);
                var tmpList = list.Select(x => new { Id = x.Id, Name = $"{x.Module} - {x.Name}" });
                var filteredList = tmpList.Where(x => string.IsNullOrWhiteSpace(searchKey) || x.Name.IndexOf(searchKey ?? "", StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                return Json(filteredList.OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            }
        }

    

        [SkipLogActionFilter]
        public async Task<ActionResult> GetReturnToAsync(string searchKey)
        {
            var username = User.Identity.Name;
            using (_systemConfigsBL = new BL.SystemConfigs(username))
            {
                var returnToStr = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.REJECT_RETURNTO).ConfigureAwait(false);
                var list = returnToStr.Split('|').Where(x => x.Split(',').Length == 2);
                var tmpList = list.Select(x => new { Id = x.Split(',')[0], Name = x.Split(',')[1] });
                var filteredList = tmpList.Where(x => string.IsNullOrWhiteSpace(searchKey) || x.Name.IndexOf(searchKey ?? "", StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                return Json(filteredList.OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            }
        }

       
        [SkipLogActionFilter]
        public async Task<ActionResult> SystemNotifications()
        {
            try
            {
                var username = User.Identity.Name;
                var list = new List<Models.SystemNotificationViewModel>();

                ViewBag.NotViewedCount = 0;

                using (_userDetailsBL = new BL.UserDetails(username))
                using (_systemNotificationsBL = new BL.SystemNotifications(username))
                {
                    var userObj = await _userDetailsBL.GetByUsernameAsync(username).ConfigureAwait(false);
                    if (userObj != null)
                    {
                        var tmpList = await _systemNotificationsBL.GetListAsync(userObj.Id).ConfigureAwait(false);
                        list = tmpList.Select(x => new Models.SystemNotificationViewModel
                        {
                            Id = x.Id,
                            UniqueKey = x.UniqueKey,
                            Url = x.Url,
                            Caption = x.Caption,
                            Created = x.Created,
                            Description = x.Description,
                            IsViewed = x.IsViewed,
                            SeenDate = x.SeenDate,
                            SystemNotification_UserDetail = x.SystemNotification_UserDetail,
                            Title = x.Title
                        }).GroupBy(x => new { x.Title, x.Caption })
                        .Select(x => x.OrderBy(o => o.IsViewed).FirstOrDefault())
                        .ToList();

                        ViewBag.NotViewedCount = list.Count(x => !x.IsViewed);
                    }
                }

                return PartialView("_SystemNotifications", list);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SystemNotificationSeen(int id)
        {
            try
            {
                var username = User.Identity.Name;
                using (_systemNotificationsBL = new BL.SystemNotifications(username))
                {
                    var obj = await _systemNotificationsBL.GetAsync(id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        obj.IsViewed = true;
                        obj.SeenDate = DateTime.Now;
                        await _systemNotificationsBL.UpdateAsync(obj).ConfigureAwait(false);

                        return Json(new { Success = true });
                    }
                }

                return Json(new { Success = false, Message = "Notification not found." });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<ActionResult> GetSystemConfigValue(string name)
        {
            try
            {
                var username = User.Identity.Name;
                using (_systemConfigsBL = new BL.SystemConfigs(username))
                {
                    var value = await _systemConfigsBL.GetValueAsync(name).ConfigureAwait(false);
                    return Json(new { Value = value }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> GetDataTypes()
        {
            var username = User.Identity.Name;
            using (_systemConfigsBL = new BL.SystemConfigs(username))
            {
                var returnToStr = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.DATA_TYPES).ConfigureAwait(false);
                var list = returnToStr.Split(',');

                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        public async Task<JsonResult> GetSuppliersForDdl(int? id = null)
        {
            var username = User.Identity.Name;
            using (var bl = new BL.Suppliers(username))
            {
                var list = new List<Models.SupplierViewModel>();
                if(id.HasValue)
                {
                    var dbObj = await bl.GetAsync(id.Value).ConfigureAwait(false);
                    if (dbObj != null)
                        list.Add(new Models.SupplierViewModel
                        {
                            Id = dbObj.Id,
                            supplier_name = dbObj.supplier_name
                        });
                }
                else
                {
                    list = (await bl.GetListAsync().ConfigureAwait(false))
                                .Select(x => new Models.SupplierViewModel
                                {
                                    Id = x.Id,
                                    supplier_name = x.supplier_name
                                }).ToList();
                }


                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        public async Task<JsonResult> GetLocationsForDdl(int? id = null)
        {
            var username = User.Identity.Name;
            using (var bl = new BL.Locations(username))
            {
                var list = new List<Models.LocationViewModel>();
                if (id.HasValue)
                {
                    var dbObj = await bl.GetAsync(id.Value).ConfigureAwait(false);
                    if (dbObj != null)
                        list.Add(new Models.LocationViewModel
                        {
                            Id = dbObj.Id,
                            location_name = dbObj.location_name
                        });
                }
                else
                {
                    list = (await bl.GetListAsync().ConfigureAwait(false))
                                .Select(x => new Models.LocationViewModel
                                {
                                    Id = x.Id,
                                    location_name = x.location_name
                                }).ToList();
                }


                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [SkipLogActionFilter]
        public async Task<JsonResult> GetProductsForDdl(int? id = null)
        {
            var username = User.Identity.Name;
            using (var bl = new BL.Products(username))
            {
                var list = new List<Models.ProductsViewModel>();
                if (id.HasValue)
                {
                    var dbObj = await bl.GetAsync(id.Value).ConfigureAwait(false);
                    if (dbObj != null)
                        list.Add(new Models.ProductsViewModel
                        {
                            Id = dbObj.Id,
                            product_name = dbObj.product_name
                        });
                }
                else
                {
                    list = (await bl.GetListAsync().ConfigureAwait(false))
                                .Select(x => new Models.ProductsViewModel
                                {
                                    Id = x.Id,
                                    product_name = x.product_name
                                }).ToList();
                }


                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }


        [SkipLogActionFilter]
        public async Task<JsonResult> GetCategoriesForDdl(int? id = null)
        {
            var username = User.Identity.Name;
            using (var bl = new BL.Categories(username))
            {
                var list = new List<Models.CategoryViewModel>();
                if (id.HasValue)
                {
                    var dbObj = await bl.GetAsync(id.Value).ConfigureAwait(false);
                    if (dbObj != null)
                        list.Add(new Models.CategoryViewModel
                        {
                            Id = dbObj.Id,
                            category_name = dbObj.category_name
                        });
                }
                else
                {
                    list = (await bl.GetListAsync().ConfigureAwait(false))
                                .Select(x => new Models.CategoryViewModel
                                {
                                    Id = x.Id,
                                    category_name = x.category_name
                                }).ToList();
                }


                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

    }
}