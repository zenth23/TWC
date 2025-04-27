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
    public class SalesOrdersController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var username = User.Identity.Name;
            try
            {
                using (var soBL = new BL.SalesOrders(username))
                using (var piBL = new BL.Product_Inventory(username))
                {
                    if (id != 0)
                    {
                        var dbObj = await soBL.GetAsync(id, "SalesOrderDetails").ConfigureAwait(false);
                        if (dbObj != null)
                        {
                            dbObj.IsDeleted = true;
                            await soBL.UpdateAsync(dbObj).ConfigureAwait(false);

                            foreach (var detailObj in dbObj.SalesOrderDetails)
                            {
                                // REMOVE TO OLD
                                var piObj = await piBL.GetAsync(detailObj.SalesOrderDetail_Product
                                                              , dbObj.location_id
                                                              , dbObj.supplier_id).ConfigureAwait(false);
                                if (piObj != null)
                                {
                                    piObj.quantity = piObj.quantity + detailObj.Qty;
                                    await piBL.UpdateAsync(piObj).ConfigureAwait(false);
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

        [HttpPost]
        public async Task<ActionResult> Save([DataSourceRequest] DataSourceRequest request, Models.SalesOrderViewModel model)
        {
            var username = User.Identity.Name;
            try
            {
                using (var soBL = new BL.SalesOrders(username))
                using (var piBL = new BL.Product_Inventory(username))
                {
                    if (model.Id == 0)
                    {
                        var obj = new TWC.IMS.Models.SalesOrderHeader();
                        obj.InvoiceNumber = model.InvoiceNumber;
                        obj.location_id = model.location_id;
                        obj.supplier_id = model.supplier_id;
                        obj.Amount = model.Cost * model.quantity;
                        obj.SalesOrderDetails = new List<TWC.IMS.Models.SalesOrderDetail>
                        {
                            new SalesOrderDetail
                            {
                                Cost = model.Cost,
                                Qty = model.quantity,
                                SalesOrderDetail_Product = model.product_id.Value,
                                Weight = model.IsGold ? model.Weight : null,
                                Created = DateTime.Now,
                                CreatedBy = username
                            }
                        };
                        var soId = await soBL.InsertAsync(obj).ConfigureAwait(false);
                        if (soId > 0)
                        {
                            foreach (var detailObj in obj.SalesOrderDetails.Take(1))
                            {
                                var piId = 0;
                                var piObj = await piBL.GetAsync(detailObj.SalesOrderDetail_Product
                                                                , obj.location_id
                                                                , obj.supplier_id)
                                                                .ConfigureAwait(false);
                                if (piObj != null)
                                {
                                    piObj.quantity = piObj.quantity - detailObj.Qty;
                                    piId = await piBL.UpdateAsync(piObj).ConfigureAwait(false);

                                }
                                else
                                {
                                    piObj = new IMS.Models.Product_Inventory
                                    {
                                        supplier_id = obj.supplier_id,
                                        location_id = obj.location_id,
                                        product_id = detailObj.SalesOrderDetail_Product,
                                        quantity = detailObj.Qty * -1
                                    };

                                    piId = await piBL.InsertAsync(piObj).ConfigureAwait(false);
                                }


                                if (piId > 0)
                                {
                                    var soObj = await soBL.GetAsync(soId).ConfigureAwait(false);
                                    if (soObj != null)
                                    {
                                        soObj.ProductInventoryId = piId;
                                        await soBL.UpdateAsync(soObj).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var dbObj = await soBL.GetAsync(model.Id, "SalesOrderDetails").ConfigureAwait(false);
                        if (dbObj != null)
                        {
                            foreach (var detailObj in dbObj.SalesOrderDetails.Take(1))
                            {
                                var isSameEntry = dbObj.location_id == model.location_id
                                    && dbObj.supplier_id == model.supplier_id
                                    && detailObj.SalesOrderDetail_Product == model.product_id;

                                if (!isSameEntry)
                                {
                                    // ADD TO NEW
                                    var piId = 0;
                                    var piObj = await piBL.GetAsync(model.product_id.Value
                                                                , model.location_id
                                                                , model.supplier_id).ConfigureAwait(false);
                                    if (piObj != null)
                                    {
                                        piObj.quantity = piObj.quantity - model.quantity;
                                        piId = await piBL.UpdateAsync(piObj).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        piObj = new IMS.Models.Product_Inventory
                                        {
                                            supplier_id = model.supplier_id,
                                            location_id = model.location_id,
                                            product_id = model.product_id.Value,
                                            quantity = model.quantity * -1
                                        };

                                        piId = await piBL.InsertAsync(piObj).ConfigureAwait(false);
                                    }

                                    // REMOVE TO OLD
                                    piObj = await piBL.GetAsync(detailObj.SalesOrderDetail_Product
                                                            , dbObj.location_id
                                                            , dbObj.supplier_id).ConfigureAwait(false);
                                    if (piObj != null)
                                    {
                                        piObj.quantity = piObj.quantity + detailObj.Qty;
                                        await piBL.UpdateAsync(piObj).ConfigureAwait(false);
                                    }


                                    if (piId > 0)
                                    {
                                        detailObj.SalesOrderDetail_Product = model.product_id.Value;
                                        detailObj.Cost = model.Cost;
                                        detailObj.Qty = model.quantity;

                                        await soBL.UpdateDetailAsync(detailObj);

                                        dbObj.supplier_id = model.supplier_id;
                                        dbObj.location_id = model.location_id;
                                        dbObj.ProductInventoryId = piId;
                                        dbObj.Amount = model.Cost * model.quantity;
                                        dbObj.InvoiceNumber = model.InvoiceNumber;

                                        await soBL.UpdateAsync(dbObj).ConfigureAwait(false);
                                    }


                                }
                                else
                                {
                                    var piObj = await piBL.GetAsync(detailObj.SalesOrderDetail_Product
                                                                    , dbObj.location_id
                                                                    , dbObj.supplier_id).ConfigureAwait(false);
                                    if (piObj != null)
                                    {
                                        var variance = model.quantity - detailObj.Qty;
                                        piObj.quantity = piObj.quantity - variance;
                                        await piBL.UpdateAsync(piObj).ConfigureAwait(false);

                                        detailObj.SalesOrderDetail_Product = model.product_id.Value;
                                        detailObj.Cost = model.Cost;
                                        detailObj.Qty = model.quantity;

                                        await soBL.UpdateDetailAsync(detailObj);

                                        dbObj.supplier_id = model.supplier_id;
                                        dbObj.location_id = model.location_id;
                                        dbObj.ProductInventoryId = piObj.Id;
                                        dbObj.Amount = model.Cost * model.quantity;
                                        dbObj.InvoiceNumber = model.InvoiceNumber;

                                        await soBL.UpdateAsync(dbObj).ConfigureAwait(false);

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


        [SkipLogActionFilter]
        public async Task<ActionResult> GetList([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                using (var soBL = new BL.SalesOrders(User.Identity.Name))
                {
                    var includes = new string[] {
                        "Location", "Supplier", "SalesOrderDetails.Product_Master"
                    };
                    var inventoryEntries = await soBL.GetListAsync(includes).ConfigureAwait(false);
                    var list = inventoryEntries.Select(entry => new SalesOrderViewModel()
                    {
                        Id = entry.Id,
                        SupplierName = entry.Supplier?.supplier_name ?? "",
                        LocationName = entry.Location?.location_name ?? "",
                        ProductName = entry.SalesOrderDetails.FirstOrDefault()?.Product_Master?.product_name ?? "",
                        supplier_id = entry.supplier_id,
                        location_id = entry.location_id,
                        InvoiceNumber = entry.InvoiceNumber,
                        Weight = entry.SalesOrderDetails.FirstOrDefault()?.Weight,
                        IsGold = entry.SalesOrderDetails.FirstOrDefault()?.Weight != null,
                        product_id = entry.SalesOrderDetails.FirstOrDefault()?.SalesOrderDetail_Product ?? 0,
                        quantity = entry.SalesOrderDetails.FirstOrDefault()?.Qty ?? 0,
                        Cost = entry.SalesOrderDetails.FirstOrDefault()?.Cost ?? 0,
                        Amount = (entry.SalesOrderDetails.FirstOrDefault()?.Qty ?? 0)
                                * (entry.SalesOrderDetails.FirstOrDefault()?.Cost ?? 0),
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
            using (ExcelPackage package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Sales Orders");
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                workSheet.Column(1).Width = 50;
                workSheet.Column(2).Width = 30;
                workSheet.Column(3).Width = 30;
                workSheet.Column(4).Width = 30;

                workSheet.Cells[1, 1].Value = "Product";
                workSheet.Cells[1, 2].Value = "Invoice Number";
                workSheet.Cells[1, 3].Value = "Weight (in grams)";
                workSheet.Cells[1, 4].Value = "Quantity";
                workSheet.Cells[1, 5].Value = "Cost";

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

                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                var fileDownloadName = "Sales Orders Template.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = fileDownloadName;

                return await Task.FromResult(fsr);
            }
        }

        public async Task<ActionResult> LoadExcelFile(IEnumerable<HttpPostedFileBase> excelUpload)
        {
            //await Task.Delay(5000);
            var list = new List<Models.SalesOrderViewModel>();
            var username = User.Identity.Name;
            if (excelUpload != null)
            {
                var excelFile = excelUpload.FirstOrDefault();
                if (excelFile != null)
                {
                    using (var productBL = new BL.Products(username))
                    using (var package = new ExcelPackage(excelFile.InputStream))
                    {
                        var products = await productBL.GetListAsync();
                        var valid = false;

                        SalesOrderViewModel itemObj = null;
                        decimal weight;
                        decimal cost;
                        int quantity;

                        var worksheet = package.Workbook.Worksheets[1];
                        var colCount = worksheet.Dimension.End.Column;  //get Column Count
                        var rowCount = worksheet.Dimension.End.Row;     //get row count
                        for (var row = 2; row <= rowCount; row++)
                        {
                            itemObj = new SalesOrderViewModel();
                            itemObj.ProductName = (worksheet.Cells[row, 1].Value?.ToString() ?? "").Trim();
                            itemObj.InvoiceNumber = (worksheet.Cells[row, 2].Value?.ToString() ?? "").Trim();

                            valid = decimal.TryParse(worksheet.Cells[row, 3].Value?.ToString() ?? "", out weight);
                            itemObj.Weight = valid ? (decimal?)weight : null;
                            itemObj.IsGold = itemObj.Weight.HasValue;

                            valid = int.TryParse(worksheet.Cells[row, 4].Value?.ToString() ?? "", out quantity);
                            itemObj.quantity = valid ? quantity : 0;

                            valid = decimal.TryParse(worksheet.Cells[row, 5].Value?.ToString() ?? "", out cost);
                            itemObj.Cost = valid ? cost : 0;

                            itemObj.Amount = itemObj.quantity * itemObj.Cost;
                            itemObj.ValidationMessage = itemObj.Amount == 0
                                                        || string.IsNullOrWhiteSpace(itemObj.ProductName)
                                                        || string.IsNullOrWhiteSpace(itemObj.InvoiceNumber)
                                                        ? "Invalid." : "";

                            if (!products.Any(x => x.product_name == itemObj.ProductName))
                            {
                                if (!string.IsNullOrWhiteSpace(itemObj.ValidationMessage))
                                    itemObj.ValidationMessage = itemObj.ValidationMessage + " Product not found.";
                                else itemObj.ValidationMessage = "Product not found.";
                            }

                            list.Add(itemObj);
                        }
                    }
                }
            }

            return Json(await Task.FromResult(list), JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Upload(Models.ExcelUploadSalesOrderViewModel[] salesOrders)
        {
            var username = User.Identity.Name;
            try
            {
                using (var productBL = new BL.Products(username))
                using (var soBL = new BL.SalesOrders(username))
                using (var piBL = new BL.Product_Inventory(username))
                {
                    var products = await productBL.GetListAsync();
                    foreach (var item in salesOrders)
                    {
                        var productObj = products.FirstOrDefault(x => x.product_name == item.ProductName);
                        if (productObj != null)
                        {

                            var obj = new TWC.IMS.Models.SalesOrderHeader();
                            obj.InvoiceNumber = item.InvoiceNumber;
                            obj.location_id = item.LocationId;
                            obj.supplier_id = item.SupplierId;
                            obj.Amount = item.Cost * item.Quantity;
                            obj.SalesOrderDetails = new List<TWC.IMS.Models.SalesOrderDetail>
                        {
                            new SalesOrderDetail
                            {
                                Cost = item.Cost,
                                Qty = item.Quantity,
                                SalesOrderDetail_Product = productObj.Id,
                                Weight = item.Weight,
                                Created = DateTime.Now,
                                CreatedBy = username
                            }
                        };
                            var soId = await soBL.InsertAsync(obj).ConfigureAwait(false);
                            if (soId > 0)
                            {
                                foreach (var detailObj in obj.SalesOrderDetails.Take(1))
                                {
                                    var piId = 0;
                                    var piObj = await piBL.GetAsync(detailObj.SalesOrderDetail_Product
                                                                    , obj.location_id
                                                                    , obj.supplier_id)
                                                                    .ConfigureAwait(false);
                                    if (piObj != null)
                                    {
                                        piObj.quantity = piObj.quantity - detailObj.Qty;
                                        piId = await piBL.UpdateAsync(piObj).ConfigureAwait(false);

                                    }
                                    else
                                    {
                                        piObj = new IMS.Models.Product_Inventory
                                        {
                                            supplier_id = obj.supplier_id,
                                            location_id = obj.location_id,
                                            product_id = detailObj.SalesOrderDetail_Product,
                                            quantity = detailObj.Qty * -1
                                        };

                                        piId = await piBL.InsertAsync(piObj).ConfigureAwait(false);
                                    }


                                    if (piId > 0)
                                    {
                                        var soObj = await soBL.GetAsync(soId).ConfigureAwait(false);
                                        if (soObj != null)
                                        {
                                            soObj.ProductInventoryId = piId;
                                            await soBL.UpdateAsync(soObj).ConfigureAwait(false);
                                        }
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