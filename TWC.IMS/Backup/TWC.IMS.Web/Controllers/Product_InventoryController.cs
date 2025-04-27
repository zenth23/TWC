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
using TWC.IMS.Common.HelperClasses;
using System.Web.Script.Serialization;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class Product_InventoryController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [Route("{controller}/Details")]
        [Route("{controller}/Details/{id}")]
        public async Task<ActionResult> Details(int? id)
        {
            using (_inventoryBL = new BL.Product_Inventory(User.Identity.Name))
            {
                var model = new Models.ProductInventoryViewModel();

                model.Id = id == null ? Convert.ToInt32(0) : Convert.ToInt32(id);
                // edit mode
                if (model.Id != 0)
                {
                    var obj = await _inventoryBL.GetAsync(model.Id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        model.Id = obj.Id;
                  
                        // get audit trail from the parent module                                 
                        model.CreatedBy = string.Format("{0} {1}", obj.CreatedBy, (obj.Created == null ? "" : "on " + obj.Created.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                        model.ModifiedBy = string.Format("{0} {1}", obj.ModifiedBy, (obj.Modified == null ? "" : "on " + obj.Modified.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                    }
                    else
                        throw new HttpException(404, $"Subscriber not found.");
                }

                ViewBag.Mode = model.Id == 0 ? "CREATE" : "EDIT";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveProductInventory([DataSourceRequest] DataSourceRequest request, ProductInventoryViewModel model)
        {
            var username = User.Identity.Name;
            try
            {
                using (_inventoryBL = new BL.Product_Inventory(username))
                {
                    Guid code = new Guid();
                    ModelState.Remove("Created");
                    ModelState.Remove("Modified");
                    ModelState.Remove("Deactivated");
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var objGetByCode = await _inventoryBL.GetByCodeAsync(code).ConfigureAwait(false);

                            if (objGetByCode == null)
                            {
                                Guid UniqueKey = Guid.NewGuid();
                                var ProdInvObj = new TWC.IMS.Models.Product_Inventory();
                             
                               ProdInvObj.UniqueKey = UniqueKey;
                               ProdInvObj.Created = DateTime.Now;
                               ProdInvObj.CreatedBy = username;
                             

                                await _inventoryBL.InsertAsync(ProdInvObj).ConfigureAwait(false);
                            }
                            else
                                return Json(new { Success = false, Message = "Product Inventory already exists!" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var bussList = await _inventoryBL.GetListAsync().ConfigureAwait(false);
                            var obj = bussList.FirstOrDefault(b => b.Id == model.Id);

                            if (bussList.FirstOrDefault(b => b.Id != model.Id && b.UniqueKey == model.UniqueKey) != null)
                                return Json(new { Success = false, Message = "Product Inventory already exists!" }, JsonRequestBehavior.AllowGet);

                            if (obj != null)
                            {
                              
                                obj.Created = obj.Created;
                                obj.Modified = DateTime.Now;
                                obj.ModifiedBy = username;
                              

                                await _inventoryBL.UpdateAsync(obj).ConfigureAwait(false);
                            }
                            else
                                return Json(new { Success = false, Message = "No Product Inventory found." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                }
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [SkipLogActionFilter]
        public async Task<ActionResult> GetProductInventory([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                using (_inventoryBL = new BL.Product_Inventory(User.Identity.Name))
                {
                    var units = await _inventoryBL.GetListAsync().ConfigureAwait(false);
                    var list = new List<ProductInventoryViewModel>();
                    foreach (var unit in units)
                    {
                        var model = new ProductInventoryViewModel()
                        {
                            Id = unit.Id,
                            Created = unit.Created,
                            CreatedBy = unit.CreatedBy,
                            Modified = unit.Modified?.DateTime.AsNullable(),
                            ModifiedBy = unit.ModifiedBy,
                           
                        };
                        list.Add(model);
                    }
                    DataSourceResult dsResult = list.ToDataSourceResult(request);
                    var serializer = new JavaScriptSerializer();
                    var result = new ContentResult();
                    serializer.MaxJsonLength = int.MaxValue;
                    result.Content = serializer.Serialize(dsResult);
                    result.ContentType = "application/json";
                    return result;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [SkipLogActionFilter]
        public async Task<ActionResult> List([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                using (_inventoryBL = new BL.Product_Inventory(User.Identity.Name))
                {
                    var appUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                    appUrl = appUrl + (appUrl[appUrl.Length - 1] == '/' ? "" : "/");

                    var includes = new string[]
                    {
                        "Location",
                        "Supplier",
                        "Product_Master.Product_Master_Images"
                    };
                    var inventories = await _inventoryBL.GetListAsync(includes).ConfigureAwait(false);
                    var list = inventories.Select(x => new Models.ProductInventoryListViewModel
                    {
                        Id = x.Id,
                        UniqueKey = x.UniqueKey,
                        ProductName = x.Product_Master?.product_name ?? "",
                        Inventory = x.quantity,
                        SupplierId = x.supplier_id,
                        LocationId = x.location_id,
                        SupplierName = x.Supplier?.supplier_name,
                        LocationName = x.Location?.location_name,
                        ImageId = x.Product_Master == null ? (int?)null
                                : (x.Product_Master.Product_Master_Images.Count == 0 ? (int?)null
                                   : x.Product_Master
                                      .Product_Master_Images
                                      .OrderByDescending(p => p.IsMain)
                                      .FirstOrDefault().Id)
                    }).ToList();

                    foreach(var item in list)
                    {
                        item.Url = $"{appUrl}products/getimage?id={item.ImageId}";
                    }

                    DataSourceResult dsResult = list.ToDataSourceResult(request);
                    var serializer = new JavaScriptSerializer();
                    var result = new ContentResult();
                    serializer.MaxJsonLength = int.MaxValue;
                    result.Content = serializer.Serialize(dsResult);
                    result.ContentType = "application/json";
                    return result;
                    
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public ActionResult InventoryGridView()
        {
            return PartialView("_InventoryGridView");
        }

        public ActionResult InventoryListView()
        {
            return PartialView("_InventoryListView");
        }


    }
}