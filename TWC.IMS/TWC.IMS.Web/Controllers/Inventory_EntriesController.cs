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
using OfficeOpenXml;
using System.IO;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class Inventory_EntriesController : BaseController
    {
        [CustomAuthorize(AccessName = "InventoryEntries.CanView")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(AccessName = "InventoryEntries.CanDelete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var username = User.Identity.Name;
            try
            {
                using (var ieBL = new BL.Inventory_Entry(username))
                using (var piBL = new BL.Product_Inventory(username))
                {
                    if (id != 0)
                    {
                        var dbObj = await ieBL.GetAsync(id).ConfigureAwait(false);
                        if (dbObj != null)
                        {
                            dbObj.deleted = true;
                            await ieBL.UpdateAsync(dbObj).ConfigureAwait(false);


                            // REMOVE TO OLD
                            var piObj = await piBL.GetAsync(dbObj.product_id, dbObj.location_id).ConfigureAwait(false);
                            if (piObj != null)
                            {
                                piObj.quantity = piObj.quantity - dbObj.quantity;
                                await piBL.UpdateAsync(piObj).ConfigureAwait(false);
                            }
                        }
                    }
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Save([DataSourceRequest] DataSourceRequest request, InventoryEntryViewModel model)
        {
            var username = User.Identity.Name;
            try
            {
                using (var ieBL = new BL.Inventory_Entry(username))
                using (var piBL = new BL.Product_Inventory(username))
                {
                    if (model.received_date < model.entry_date)
                        throw new Exception("Received date must be later than Entry Date");

                    if (model.Id == 0 && HttpContext.HasPermission("InventoryEntries.CanAdd"))
                    {
                        var obj = new TWC.IMS.Models.Inventory_Entry();
                        obj.product_id = model.product_id;
                        obj.location_id = model.location_id;
                        obj.quantity = model.quantity;
                        obj.entry_date = model.entry_date;
                        obj.category_id = model.category_id;
                        obj.received_date = model.received_date;
                        obj.remarks = model.remarks;

                        var ieId = await ieBL.InsertAsync(obj).ConfigureAwait(false);
                        if (ieId > 0)
                        {
                            var piId = 0;
                            var piObj = await piBL.GetAsync(obj.product_id, obj.location_id).ConfigureAwait(false);
                            if (piObj != null)
                            {
                                piObj.quantity = piObj.quantity + obj.quantity;
                                piId = await piBL.UpdateAsync(piObj).ConfigureAwait(false);

                            }
                            else
                            {
                                piObj = new IMS.Models.Product_Inventory
                                {

                                    location_id = obj.location_id,
                                    product_id = obj.product_id,
                                    quantity = obj.quantity
                                };

                                piId = await piBL.InsertAsync(piObj).ConfigureAwait(false);
                            }


                            if (piId > 0)
                            {
                                var inventoryEntryObj = await ieBL.GetAsync(ieId).ConfigureAwait(false);
                                if (inventoryEntryObj != null)
                                {
                                    inventoryEntryObj.inventory_id = piId;
                                    await ieBL.UpdateAsync(inventoryEntryObj).ConfigureAwait(false);
                                }
                            }
                        }
                    }
                    else if (model.Id != 0 && HttpContext.HasPermission("InventoryEntries.CanEdit"))
                    {
                        var dbObj = await ieBL.GetAsync(model.Id).ConfigureAwait(false);
                        if (dbObj != null)
                        {
                            var isSameEntry = dbObj.location_id == model.location_id
                                && dbObj.product_id == model.product_id;

                            if (!isSameEntry)
                            {
                                // ADD TO NEW
                                var piId = 0;
                                var piObj = await piBL.GetAsync(model.product_id, model.location_id).ConfigureAwait(false);
                                if (piObj != null)
                                {
                                    piObj.quantity = piObj.quantity + model.quantity;
                                    piId = await piBL.UpdateAsync(piObj).ConfigureAwait(false);
                                }
                                else
                                {
                                    piObj = new IMS.Models.Product_Inventory
                                    {

                                        location_id = model.location_id,
                                        product_id = model.product_id,
                                        quantity = model.quantity
                                    };

                                    piId = await piBL.InsertAsync(piObj).ConfigureAwait(false);
                                }

                                // REMOVE TO OLD
                                piObj = await piBL.GetAsync(dbObj.product_id, dbObj.location_id).ConfigureAwait(false);
                                if (piObj != null)
                                {
                                    piObj.quantity = piObj.quantity - model.quantity;
                                    await piBL.UpdateAsync(piObj).ConfigureAwait(false);
                                }


                                if (piId > 0)
                                {
                                    dbObj.product_id = model.product_id;
                                    dbObj.location_id = model.location_id;
                                    dbObj.quantity = model.quantity;
                                    dbObj.entry_date = model.entry_date;
                                    dbObj.category_id = model.category_id;
                                    dbObj.received_date = model.received_date;
                                    dbObj.remarks = model.remarks;
                                    dbObj.inventory_id = piId;
                                    await ieBL.UpdateAsync(dbObj).ConfigureAwait(false);
                                }


                            }
                            else
                            {
                                var piObj = await piBL.GetAsync(dbObj.product_id).ConfigureAwait(false);
                                if (piObj != null)
                                {
                                    var variance = model.quantity - dbObj.quantity;
                                    piObj.quantity = piObj.quantity + variance;
                                    await piBL.UpdateAsync(piObj).ConfigureAwait(false);

                                    dbObj.quantity = model.quantity;
                                    dbObj.entry_date = model.entry_date;
                                    dbObj.category_id = model.category_id;
                                    dbObj.received_date = model.received_date;
                                    dbObj.remarks = model.remarks;
                                    await ieBL.UpdateAsync(dbObj).ConfigureAwait(false);

                                }
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Unauthorized." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(AccessName = "InventoryEntries.CanView")]
        [SkipLogActionFilter]
        public async Task<ActionResult> GetList([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                using (var inventoryEntryBL = new BL.Inventory_Entry(User.Identity.Name))
                {
                    var includes = new string[] {
                        "Location","Product_Master", "Category"
                    };
                    var inventoryEntries = await inventoryEntryBL.GetListAsync(includes).ConfigureAwait(false);
                    var list = inventoryEntries.Select(entry => new InventoryEntryViewModel()
                    {
                        Id = entry.Id,
                        LocationName = entry.Location?.location_name ?? "",
                        CategoryName = entry.Category?.category_name ?? "",
                        ProductName = entry.Product_Master?.product_name ?? "",
                        location_id = entry.location_id,
                        category_id = entry.category_id,
                        product_id = entry.product_id,
                        quantity = entry.quantity,
                        remarks = entry.remarks,
                        entry_date = entry.entry_date,
                        received_date = entry.received_date,
                        Created = entry.Created,
                        CreatedBy = entry.CreatedBy,
                        Modified = entry.Modified?.DateTime.AsNullable(),
                        ModifiedBy = entry.ModifiedBy
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

        [SkipLogActionFilter]
        public async Task<ActionResult> DownloadTemplate()
        {
            var username = User.Identity.Name;
            using (var productsBL = new BL.Products(username))
            using (var catBL = new BL.Categories(username))
            using (ExcelPackage package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Inventory Entries");
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                workSheet.Column(1).Width = 50;
                workSheet.Column(2).Width = 30;
                workSheet.Column(3).Width = 30;
                workSheet.Column(4).Width = 30;
                workSheet.Column(5).Width = 30;
                workSheet.Column(6).Width = 30;

                workSheet.Cells[1, 1].Value = "Product";
                workSheet.Cells[1, 2].Value = "Category";
                workSheet.Cells[1, 3].Value = "Quantity";
                workSheet.Cells[1, 4].Value = "Entry Date";
                workSheet.Cells[1, 5].Value = "Received Date";
                workSheet.Cells[1, 6].Value = "Remarks";

                // PRODUCTS
                var lookupSheet = package.Workbook.Worksheets.Add("Products_Lookup");
                lookupSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

                var list = await productsBL.GetListAsync();
                var count = 1;
                foreach (var item in list)
                {
                    lookupSheet.Cells[count, 1].Value = item.product_name;
                    count++;
                }

                var start = lookupSheet.Cells[1, 1].ToString();
                var end = lookupSheet.Cells[count, 1].ToString();
                start = "$" + start.Substring(0, 1) + "$" + start.Substring(1);
                end = "$" + end.Substring(0, 1) + "$" + end.Substring(1);

                var range = start + ":" + end;
                var rangeLookUp = ExcelRange.GetAddress(2, 1, ExcelPackage.MaxRows, 1);

                var rangeListExcelDropDown = workSheet.DataValidations.AddListValidation(rangeLookUp);
                rangeListExcelDropDown.Formula.ExcelFormula = "Products_Lookup!" + range.ToString();


                // CATEGORIES
                var catSheet = package.Workbook.Worksheets.Add("Categories_Lookup");
                catSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

                var catList = await catBL.GetListAsync();
                var catCount = 1;
                foreach (var item in catList)
                {
                    catSheet.Cells[catCount, 1].Value = item.category_name;
                    catCount++;
                }

                var catStart = catSheet.Cells[1, 1].ToString();
                var catEnd = catSheet.Cells[catCount, 1].ToString();
                catStart = "$" + catStart.Substring(0, 1) + "$" + catStart.Substring(1);
                catEnd = "$" + catEnd.Substring(0, 1) + "$" + catEnd.Substring(1);

                var catRange = catStart + ":" + catEnd;
                var catRangeLookUp = ExcelRange.GetAddress(2, 2, ExcelPackage.MaxRows, 2);

                var catRangeListExcelDropDown = workSheet.DataValidations.AddListValidation(catRangeLookUp);
                catRangeListExcelDropDown.Formula.ExcelFormula = "Categories_Lookup!" + catRange.ToString();


                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                var fileDownloadName = "Inventory Entries Template.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = fileDownloadName;

                return await Task.FromResult(fsr);
            }
        }

        public async Task<ActionResult> LoadExcelFile(IEnumerable<HttpPostedFileBase> excelUpload)
        {
            //await Task.Delay(5000);
            var list = new List<Models.InventoryEntryViewModel>();
            var username = User.Identity.Name;
            if (excelUpload != null)
            {
                var excelFile = excelUpload.FirstOrDefault();
                if (excelFile != null)
                {
                    using (var catBL = new BL.Categories(username))
                    using (var productBL = new BL.Products(username))
                    using (var package = new ExcelPackage(excelFile.InputStream))
                    {
                        var products = await productBL.GetListAsync();
                        var categories = await catBL.GetListAsync();
                        var valid = false;

                        InventoryEntryViewModel itemObj = null;
                        DateTime entryDate;
                        DateTime receivedDate;
                        int quantity;

                        var worksheet = package.Workbook.Worksheets[1];
                        var colCount = worksheet.Dimension.End.Column;
                        var rowCount = worksheet.Dimension.End.Row;
                        for (var row = 2; row <= rowCount; row++)
                        {
                            itemObj = new InventoryEntryViewModel();
                            itemObj.ProductName = (worksheet.Cells[row, 1].Value?.ToString() ?? "").Trim();
                            itemObj.CategoryName = (worksheet.Cells[row, 2].Value?.ToString() ?? "").Trim();

                            valid = int.TryParse(worksheet.Cells[row, 3].Value?.ToString() ?? "", out quantity);
                            itemObj.quantity = valid ? (int)quantity : 0;

                            valid = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString() ?? "", out entryDate);
                            itemObj.entry_date = valid ? (DateTime?)entryDate : null;

                            valid = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString() ?? "", out receivedDate);
                            itemObj.received_date = valid ? (DateTime?)receivedDate : null;

                            itemObj.remarks = (worksheet.Cells[row, 6].Value?.ToString() ?? "").Trim();
                            itemObj.ValidationMessage = string.IsNullOrWhiteSpace(itemObj.ProductName)
                                                        || string.IsNullOrWhiteSpace(itemObj.CategoryName)
                                                        || !itemObj.entry_date.HasValue
                                                        || !itemObj.entry_date.HasValue
                                                        ? "Invalid." : "";

                            if (!products.Any(x => x.product_name == itemObj.ProductName))
                            {
                                var href = Url.Action("index", "products");
                                var aLink = $"<a href='{href}' target='blank' style='color:blue; text-decoration: underline'>here</a>";

                                if (!string.IsNullOrWhiteSpace(itemObj.ValidationMessage))
                                    itemObj.ValidationMessage = itemObj.ValidationMessage + $" Product not found. Please click {aLink} to add new product.";
                                else itemObj.ValidationMessage = $"Product not found. Please click here {aLink} to add new product.";
                            }

                            if (!categories.Any(x => x.category_name == itemObj.CategoryName))
                            {
                                if (!string.IsNullOrWhiteSpace(itemObj.ValidationMessage))
                                    itemObj.ValidationMessage = itemObj.ValidationMessage + " Category not found.";
                                else itemObj.ValidationMessage = "Category not found.";
                            }

                            list.Add(itemObj);
                        }
                    }
                }
            }

            return Json(await Task.FromResult(list), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(AccessName = "InventoryEntries.CanAdd")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Upload(Models.ExcelUploadInventoryEntryViewModel[] inventoryEntries)
        {
            var username = User.Identity.Name;
            try
            {
                using (var categoryBL = new BL.Categories(username))
                using (var productBL = new BL.Products(username))
                using (var ieBL = new BL.Inventory_Entry(username))
                using (var piBL = new BL.Product_Inventory(username))
                {
                    var products = await productBL.GetListAsync();
                    var categories = await categoryBL.GetListAsync();
                    foreach (var itemObj in inventoryEntries)
                    {
                        if (itemObj.ReceivedDate > itemObj.EntryDate)
                        {
                            return Json(new { Success = false, Message = "Received date must be later than Entry Date" }, JsonRequestBehavior.AllowGet);
                            //throw new Exception("Received date must be later than Entry Date");
                        }
                        var productObj = products.FirstOrDefault(x => x.product_name == itemObj.ProductName);
                        var catObj = categories.FirstOrDefault(x => x.category_name == itemObj.CategoryName);
                        if (productObj != null && catObj != null)
                        {
                            var obj = new TWC.IMS.Models.Inventory_Entry();
                            obj.product_id = productObj.Id;
                            obj.category_id = catObj.Id;
                            obj.location_id = itemObj.LocationId;
                            obj.quantity = itemObj.Quantity;
                            obj.entry_date = itemObj.EntryDate;
                            obj.received_date = itemObj.ReceivedDate;
                            obj.remarks = itemObj.Remarks;

                            var ieId = await ieBL.InsertAsync(obj).ConfigureAwait(false);
                            if (ieId > 0)
                            {
                                var piId = 0;
                                var piObj = await piBL.GetAsync(obj.product_id, obj.location_id).ConfigureAwait(false);
                                if (piObj != null)
                                {
                                    piObj.quantity = piObj.quantity + obj.quantity;
                                    piId = await piBL.UpdateAsync(piObj).ConfigureAwait(false);

                                }
                                else
                                {
                                    piObj = new IMS.Models.Product_Inventory
                                    {
                                        location_id = obj.location_id,
                                        product_id = obj.product_id,
                                        quantity = obj.quantity
                                    };

                                    piId = await piBL.InsertAsync(piObj).ConfigureAwait(false);
                                }


                                if (piId > 0)
                                {
                                    var inventoryEntryObj = await ieBL.GetAsync(ieId).ConfigureAwait(false);
                                    if (inventoryEntryObj != null)
                                    {
                                        inventoryEntryObj.inventory_id = piId;
                                        await ieBL.UpdateAsync(inventoryEntryObj).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}