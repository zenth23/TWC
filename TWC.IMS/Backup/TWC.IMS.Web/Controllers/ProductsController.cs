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
using System.IO;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class ProductsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inventory()
        {
            return View();
        }

        [Route("{controller}/Details")]
        [Route("{controller}/Details/{id}")]
        public async Task<ActionResult> Details(int? id)
        {
            using (_productsBL = new BL.Products(User.Identity.Name))
            {
                var model = new Models.ProductsViewModel();

                model.Id = id == null ? Convert.ToInt32(0) : Convert.ToInt32(id);
                // edit mode
                if (model.Id != 0)
                {
                    var obj = await _productsBL.GetAsync(model.Id).ConfigureAwait(false);
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
        public async Task<ActionResult> SaveProduct([DataSourceRequest] DataSourceRequest request, ProductsViewModel model)
        {
            var username = User.Identity.Name;
            try
            {
                using (_productsBL = new BL.Products(username))
                {
                    Guid code = new Guid();
                    ModelState.Remove("Created");
                    ModelState.Remove("Modified");
                    ModelState.Remove("Deactivated");
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var objGetByCode = await _productsBL.GetByCodeAsync(code).ConfigureAwait(false);

                            if (objGetByCode == null)
                            {
                                Guid UniqueKey = Guid.NewGuid();
                                var ProdInvObj = new TWC.IMS.Models.Product_Master();
                             
                               ProdInvObj.UniqueKey = UniqueKey;
                               ProdInvObj.Created = DateTime.Now;
                               ProdInvObj.CreatedBy = username;
                             

                                await _productsBL.InsertAsync(ProdInvObj).ConfigureAwait(false);
                            }
                            else
                                return Json(new { Success = false, Message = "Product already exists!" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var bussList = await _productsBL.GetListAsync().ConfigureAwait(false);
                            var obj = bussList.FirstOrDefault(b => b.Id == model.Id);

                            if (bussList.FirstOrDefault(b => b.Id != model.Id && b.UniqueKey == model.UniqueKey) != null)
                                return Json(new { Success = false, Message = "Product already exists!" }, JsonRequestBehavior.AllowGet);

                            if (obj != null)
                            {
                              
                                obj.Created = obj.Created;
                                obj.Modified = DateTime.Now;
                                obj.ModifiedBy = username;
                              

                                await _productsBL.UpdateAsync(obj).ConfigureAwait(false);
                            }
                            else
                                return Json(new { Success = false, Message = "No Product found." }, JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> GetProduct([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                using (_productsBL = new BL.Products(User.Identity.Name))
                {
                    var units = await _productsBL.GetListAsync().ConfigureAwait(false);
                    var list = new List<ProductsViewModel>();
                    foreach (var unit in units)
                    {
                        var model = new ProductsViewModel()
                        {
                            Id = unit.Id,
                            product_name = unit.product_name,
                            product_type = unit.product_type,
                            karat = unit.karat,
                            weight = unit.weight,
                            material = unit.material,
                            gemstones = unit.gemstones,
                            retail_price = unit.retail_price,
                            selling_price = unit.selling_price,
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

        public async Task<ActionResult> AddImage(IEnumerable<HttpPostedFileBase> imageUpload, int id)
        {
            var username = User.Identity.Name;
            if (imageUpload != null)
            {
                var appData = Server.MapPath("~/App_Data");
                var physicalPath = Path.Combine(appData, id.ToString());

                if (!Directory.Exists(appData))
                    Directory.CreateDirectory(appData);

                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);

                foreach (var file in imageUpload)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fullPath = Path.Combine(physicalPath, fileName);

                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                    

                    await Task.Delay(100); 

                    file.SaveAs(fullPath);

                    using(var bl = new BL.Product_Master_Image(username))
                    {
                        var dbObj = await bl.GetByFileNameAsync(fileName, id);
                        if (dbObj == null)
                            await bl.InsertAsync(new IMS.Models.Product_Master_Image
                            {
                                FileName = fileName,
                                FilePath = fullPath,
                                product_id = id
                            });
                    }
                }
            }
            
            return Content("");
        }

        public async Task<ActionResult> RemoveImage(string[] fileNames, int id)
        {

            var username = User.Identity.Name;
            if (fileNames != null)
            {
                var appData = Server.MapPath("~/App_Data");
                var physicalPath = Path.Combine(appData, id.ToString());

                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var fullPath = Path.Combine(physicalPath, fileName);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);

                        using (var bl = new BL.Product_Master_Image(username))
                        {
                            var dbObj = await bl.GetByFileNameAsync(fileName, id);
                            if (dbObj != null)
                                await bl.DeleteAsync(dbObj.Id);
                        }
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }


        [SkipLogActionFilter]
        public async Task<ActionResult> GetProductImages([DataSourceRequest]DataSourceRequest request, int id)
        {
            try
            {
                using (var bl = new BL.Product_Master_Image(User.Identity.Name))
                {
                    if (id == 0)
                        return Json(new List<ProductMasterImageViewModel>(), JsonRequestBehavior.AllowGet);

                    var dbList = await bl.GetListAsync(id).ConfigureAwait(false);
                    var appUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                    appUrl = appUrl + (appUrl[appUrl.Length - 1] == '/' ? "" : "/");
                    var list = dbList.Select(entry => new ProductMasterImageViewModel()
                    {
                        Id = entry.Id,
                        FileName = entry.FileName,
                        FilePath = $"{appUrl}products/getimage?id={entry.Id}",
                        product_id = entry.product_id,
                        UniqueKey = entry.UniqueKey,
                        IsMain = entry.IsMain
                    });

                    var dsResult = list.ToDataSourceResult(request);
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = int.MaxValue;

                    var result = new ContentResult();
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

        public async Task<ActionResult> GetImage(int id)
        {
            try
            {
                using (var bl = new BL.Product_Master_Image(User.Identity.Name))
                {
                    var imageObj = await bl.GetAsync(id);
                    if (imageObj != null)
                    {
                        var fileBytes = System.IO.File.ReadAllBytes(imageObj.FilePath);
                        var contentType = MimeMapping.GetMimeMapping(imageObj.FilePath);

                        return File(fileBytes, contentType, imageObj.FileName);
                    }
                    return Content("");
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveImage(int id)
        {
            try
            {
                var username = User.Identity.Name;
                if (id != 0)
                {
                    using (var bl = new BL.Product_Master_Image(username))
                    {
                        var dbObj = await bl.GetAsync(id);
                        if (dbObj != null)
                            await bl.DeleteAsync(dbObj.Id);

                        if (System.IO.File.Exists(dbObj.FilePath))
                            System.IO.File.Delete(dbObj.FilePath);
                    }
                }
                return Json(new { Success = true });
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetAsPrevImage(int id)
        {
            try
            {
                var username = User.Identity.Name;
                if (id != 0)
                {
                    using (var bl = new BL.Product_Master_Image(username))
                    {
                        var dbObj = await bl.GetAsync(id);
                        if (dbObj != null)
                        {
                            dbObj.IsMain = true;
                            await bl.UpdateAsync(dbObj);

                            var list = await bl.GetListAsync(dbObj.product_id);
                            foreach(var item in list.Where(x => x.Id != dbObj.Id))
                            {
                                item.IsMain = false;
                                await bl.UpdateAsync(item);
                            }
                        }
                    }
                }
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}