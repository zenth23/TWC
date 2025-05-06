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
    public class SalesOrdersDetailsController : BaseController
    {

        [CustomAuthorize(AccessName = "SalesOrdersDetails.CanView")]
        public ActionResult Index()
        {
            var model = new SalesOrderViewModel();
            return View(model);
            //return View();
        }



        public ActionResult NewDetails()
        {
            var model = new SalesOrderViewModel();
            return View("Details", model);
            //return View();
        }

        public async Task<ActionResult> Details(int? id)
        {
            var model = new SalesOrderViewModel();

            if (id.HasValue) // Edit Mode
            {
                using (var soBL = new BL.SalesOrders(User.Identity.Name))
                {
                    var order = await soBL.GetAsync(id.Value, "SalesOrderDetails").ConfigureAwait(false);
                    if (order == null)
                    {
                        return HttpNotFound();
                    }

                    model = new SalesOrderViewModel
                    {
                        Id = order.Id,
                        InvoiceNumber = order.InvoiceNumber,
                        location_id = order.location_id,
                        SalesType_id = order.SalesType_id,
                        SalesOrderDetails = order.SalesOrderDetails?.Select(d => new SalesOrderDetail
                        {
                            SalesOrderDetail_Product = d.SalesOrderDetail_Product,
                            Qty = d.Qty,
                            Weight = d.Weight,
                            Cost = d.Cost,
                            isGold = d.isGold,
                            
                        }).ToList()
                    };
                }
            }
            else // New Entry Mode
            {
                model = new SalesOrderViewModel();
                return View("Details", model);
            }

            return View(model);
        }


        [CustomAuthorize(AccessName = "SalesOrdersDetails.CanDelete")]
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
                                                              , dbObj.location_id).ConfigureAwait(false);
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
        public async Task<ActionResult> Save_Old([DataSourceRequest] DataSourceRequest request, Models.SalesOrderViewModel model)
        {
            var username = User.Identity.Name;
            try
            {
                using (var soBL = new BL.SalesOrders(username))
                using (var piBL = new BL.Product_Inventory(username))
                {
                    if (model.Id == 0 && HttpContext.HasPermission("SalesOrders.CanAdd"))
                    {
                        var amnt = model.IsGold ? model.Cost * model.quantity : model.Cost * model.Weight;
                        var obj = new TWC.IMS.Models.SalesOrderHeader();
                        obj.InvoiceNumber = model.InvoiceNumber;
                        obj.location_id = model.location_id;
                        obj.SalesType_id = model.SalesType_id;
                        obj.Amount = amnt.HasValue ? amnt.Value : 0;
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
                                                                , obj.location_id)
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
                    else if (model.Id != 0 && HttpContext.HasPermission("SalesOrders.CanEdit"))
                    {
                        var dbObj = await soBL.GetAsync(model.Id, "SalesOrderDetails").ConfigureAwait(false);
                        if (dbObj != null)
                        {
                            foreach (var detailObj in dbObj.SalesOrderDetails.Take(1))
                            {
                                var isSameEntry = dbObj.location_id == model.location_id
                                    && dbObj.SalesType_id == model.SalesType_id
                                    && detailObj.SalesOrderDetail_Product == model.product_id && detailObj.isGold == model.IsGold;

                                if (!isSameEntry)
                                {
                                    // ADD TO NEW
                                    var piId = 0;
                                    var piObj = await piBL.GetAsync(model.product_id.Value
                                                                , model.location_id).ConfigureAwait(false);
                                    if (piObj != null)
                                    {
                                        piObj.quantity = piObj.quantity - model.quantity;
                                        piId = await piBL.UpdateAsync(piObj).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        piObj = new IMS.Models.Product_Inventory
                                        {

                                            location_id = model.location_id,
                                            product_id = model.product_id.Value,
                                            quantity = model.quantity * -1
                                        };

                                        piId = await piBL.InsertAsync(piObj).ConfigureAwait(false);
                                    }

                                    // REMOVE TO OLD
                                    piObj = await piBL.GetAsync(detailObj.SalesOrderDetail_Product
                                                            , dbObj.location_id).ConfigureAwait(false);
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
                                        detailObj.isGold = model.IsGold;
                                        await soBL.UpdateDetailAsync(detailObj);

                                        dbObj.SalesType_id = model.SalesType_id;
                                        dbObj.location_id = model.location_id;
                                        dbObj.ProductInventoryId = piId;


                                        var amnt = !model.IsGold ? model.Cost * model.quantity : model.Cost * model.Weight;

                                        dbObj.Amount = amnt.HasValue ? amnt.Value : 0;
                                        dbObj.InvoiceNumber = model.InvoiceNumber;

                                        await soBL.UpdateAsync(dbObj).ConfigureAwait(false);
                                    }


                                }
                                else
                                {
                                    var piObj = await piBL.GetAsync(detailObj.SalesOrderDetail_Product
                                                                    , dbObj.location_id).ConfigureAwait(false);
                                    if (piObj != null)
                                    {
                                        var amnt = !model.IsGold ? model.Cost * model.quantity : model.Cost * model.Weight;

                                        var variance = model.quantity - detailObj.Qty;
                                        piObj.quantity = piObj.quantity - variance;
                                        await piBL.UpdateAsync(piObj).ConfigureAwait(false);

                                        detailObj.SalesOrderDetail_Product = model.product_id.Value;
                                        detailObj.Cost = model.Cost;
                                        detailObj.Qty = model.quantity;
                                        detailObj.Weight = model.Weight;
                                        detailObj.isGold = model.IsGold;
                                        await soBL.UpdateDetailAsync(detailObj);

                                        dbObj.SalesType_id = model.SalesType_id;
                                        dbObj.location_id = model.location_id;
                                        dbObj.ProductInventoryId = piObj.Id;
                                        dbObj.Amount = amnt.HasValue ? amnt.Value : 0;
                                        dbObj.InvoiceNumber = model.InvoiceNumber;

                                        await soBL.UpdateAsync(dbObj).ConfigureAwait(false);

                                    }
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

        [HttpPost]
        public async Task<ActionResult> Save_Quats([DataSourceRequest] DataSourceRequest request, SalesOrderViewModel model)
        {
            if (model == null || model.location_id <= 0)
            {
                return Json(new { Success = false, Message = "Invalid Location. Please select a valid location." });
            }

            if (model.SalesType_id <= 0)
            {
                return Json(new { Success = false, Message = "Invalid Sales Type. Please select a valid sales type." });
            }

            if (model.SalesOrderDetails == null || !model.SalesOrderDetails.Any())
            {
                return Json(new { Success = false, Message = "At least one product must be added to the order." });
            }

            try
            {
                using (var soBL = new BL.SalesOrders(User.Identity.Name))
                {
                    var salesOrder = new SalesOrderHeader
                    {
                        InvoiceNumber = model.InvoiceNumber,
                        location_id = model.location_id,
                        SalesType_id = model.SalesType_id,
                        Amount = (decimal)model.SalesOrderDetails.Sum(d => d.isGold ? d.Cost * d.Weight : d.Cost * d.Qty),
                        Created = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        SalesOrderDetails = model.SalesOrderDetails.Select(detail => new SalesOrderDetail
                        {
                            SalesOrderDetail_Product = detail.SalesOrderDetail_Product,
                            Cost = detail.Cost,
                            Qty = detail.Qty,
                            Weight = detail.isGold ? detail.Weight : null,
                            isGold = detail.isGold,
                            Created = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        }).ToList()
                    };

                    var soId = await soBL.InsertAsync(salesOrder).ConfigureAwait(false);
                    if (soId > 0)
                    {
                        return Json(new { Success = true });
                    }
                }
                return Json(new { Success = false, Message = "Failed to save sales order." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Save([DataSourceRequest] DataSourceRequest request, SalesOrderViewModel model)
        {
            if (model == null || model.location_id <= 0)
            {
                return Json(new { Success = false, Message = "Invalid Location. Please select a valid location." });
            }

            if (model.SalesType_id <= 0)
            {
                return Json(new { Success = false, Message = "Invalid Sales Type. Please select a valid sales type." });
            }

            if (model.SalesOrderDetails == null || !model.SalesOrderDetails.Any())
            {
                return Json(new { Success = false, Message = "At least one product must be added to the order." });
            }

            try
            {
                using (var soBL = new BL.SalesOrders(User.Identity.Name))
                using (var piBL = new BL.Product_Inventory(User.Identity.Name)) // Inventory update
                {
                    var salesOrder = new SalesOrderHeader
                    {
                        InvoiceNumber = model.InvoiceNumber,
                        location_id = model.location_id,
                        SalesType_id = model.SalesType_id,
                        Amount = (decimal)model.SalesOrderDetails.Sum(d => d.isGold ? d.Cost * d.Weight : d.Cost * d.Qty),
                        Created = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        SalesOrderDetails = new List<SalesOrderDetail>()
                    };

                    // Insert Sales Order Header
                    var soId = await soBL.InsertAsync(salesOrder).ConfigureAwait(false);

                    if (soId > 0)
                    {
                        // Retrieve the newly created Sales Order
                        var createdOrder = await soBL.GetAsync(soId).ConfigureAwait(false);

                        foreach (var detail in model.SalesOrderDetails)
                        {
                            var orderDetail = new SalesOrderDetail
                            {
                                SalesOrderDetail_Product = detail.SalesOrderDetail_Product,
                                SalesOrderDetail_SalesOrderHeader = soId, // Ensure it links to the correct header
                                Cost = detail.Cost,
                                Qty = detail.Qty,
                                Weight = detail.isGold ? detail.Weight : null,
                                isGold = detail.isGold,
                                Created = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            };

                            // Insert Sales Order Detail
                            await soBL.InsertDetailAsync(orderDetail).ConfigureAwait(false);

                            // Update product inventory (reduce stock)
                            var piObj = await piBL.GetAsync(detail.SalesOrderDetail_Product, model.location_id).ConfigureAwait(false);
                            if (piObj != null)
                            {
                                piObj.quantity -= detail.Qty; // Reduce stock
                                await piBL.UpdateAsync(piObj).ConfigureAwait(false);
                            }
                            else
                            {
                                // If inventory entry does not exist, create it
                                var newInventory = new IMS.Models.Product_Inventory
                                {
                                    location_id = model.location_id,
                                    product_id = detail.SalesOrderDetail_Product,
                                    quantity = detail.Qty * -1
                                };

                                await piBL.InsertAsync(newInventory).ConfigureAwait(false);
                            }

                            createdOrder.SalesOrderDetails.Add(orderDetail);
                        }

                        // Update sales order with the added details
                        await soBL.UpdateAsync(createdOrder).ConfigureAwait(false);

                        return Json(new { Success = true });
                    }
                }
                return Json(new { Success = false, Message = "Failed to save sales order." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }




        [CustomAuthorize(AccessName = "SalesOrdersDetails.CanView")]
        [SkipLogActionFilter]
        public async Task<ActionResult> GetList([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                using (var soBL = new BL.SalesOrders(User.Identity.Name))
                {
                    var includes = new string[] {
                        "Location", "SalesType", "SalesOrderDetails.Product_Master"
                    };
                    var inventoryEntries = await soBL.GetListAsync(includes).ConfigureAwait(false);
                    var list = inventoryEntries.Select(entry => new SalesOrderViewModel()
                    {
                        Id = entry.Id,
                        SalesTypeName = entry.SalesType?.SalesType_name ?? "",
                        LocationName = entry.Location?.location_name ?? "",
                        ProductName = entry.SalesOrderDetails.FirstOrDefault()?.Product_Master?.product_name ?? "",
                        SalesType_id = entry.SalesType_id,
                        location_id = entry.location_id,
                        InvoiceNumber = entry.InvoiceNumber,
                        Weight = entry.SalesOrderDetails.FirstOrDefault()?.Weight,
                        IsGold = entry.SalesOrderDetails.FirstOrDefault().isGold,
                        product_id = entry.SalesOrderDetails.FirstOrDefault()?.SalesOrderDetail_Product ?? 0,
                        quantity = entry.SalesOrderDetails.FirstOrDefault()?.Qty ?? 0,
                        Cost = entry.SalesOrderDetails.FirstOrDefault()?.Cost ?? 0,
                        Amount = entry.Amount,
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


        [CustomAuthorize(AccessName = "SalesOrdersDetails.CanView")]
        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            try
            {
                using (var soBL = new BL.SalesOrders(User.Identity.Name))
                {
                    var includes = new string[] { "Location", "SalesType"};
                    var orders = await soBL.GetListAsync(includes).ConfigureAwait(false);

                    var list = orders.Select(entry => new
                    {
                        entry.Id,
                        entry.InvoiceNumber,
                        Amount = entry.Amount.ToString("F2"),
                        SalesTypeName = entry.SalesType?.SalesType_name ?? "N/A",
                        LocationName = entry.Location?.location_name ?? "N/A"
                    });

                    return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //[SkipLogActionFilter]
        //public async Task<ActionResult> GetOrders([DataSourceRequest]DataSourceRequest request)
        //{
        //    try
        //    {
        //        using (var soBL = new BL.SalesOrders(User.Identity.Name))
        //        {
        //            var includes = new string[] {
        //                "Location", "SalesType", "SalesOrderDetails.Product_Master"
        //            };
        //            var inventoryEntries = await soBL.GetListAsync(includes).ConfigureAwait(false);
        //            var list = inventoryEntries.Select(entry => new SalesViewModel()
        //            {
        //                Id = entry.Id,
        //                Amount = entry.Amount.ToString(),
        //                LocationName = entry.Location?.location_name,
        //                SalesTypeName = entry.SalesType?.SalesType_name
        //            });

        //            var dsResult = list.ToDataSourceResult(request);
        //            var serializer = new JavaScriptSerializer();
        //            serializer.MaxJsonLength = int.MaxValue;

        //            var result = new ContentResult();
        //            result.Content = serializer.Serialize(dsResult);
        //            result.ContentType = "application/json";

        //            return result;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }

        //}



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
                workSheet.Column(5).Width = 30;
                workSheet.Column(6).Width = 30;

                workSheet.Cells[1, 1].Value = "Product";
                workSheet.Cells[1, 2].Value = "Invoice Number";
                workSheet.Cells[1, 3].Value = "IsGold";
                workSheet.Cells[1, 4].Value = "Weight (in grams)";
                workSheet.Cells[1, 5].Value = "Quantity";
                workSheet.Cells[1, 6].Value = "Cost";

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


                var isGoldLookUpSheet = package.Workbook.Worksheets.Add("IsGold");
                isGoldLookUpSheet.Hidden = eWorkSheetHidden.Hidden;

                isGoldLookUpSheet.Cells[1, 1].Value = "TRUE";
                isGoldLookUpSheet.Cells[2, 1].Value = "FALSE";
                var IsGoldStart = isGoldLookUpSheet.Cells[1, 1].ToString();
                var IsGoldEnd = isGoldLookUpSheet.Cells[2, 1].ToString();
                IsGoldStart = "$" + IsGoldStart.Substring(0, 1) + "$" + IsGoldStart.Substring(1);
                IsGoldEnd = "$" + IsGoldEnd.Substring(0, 1) + "$" + IsGoldEnd.Substring(1);

                var IsGoldRange = IsGoldStart + ":" + IsGoldEnd;
                var IsGoldRangeLookUp = ExcelRange.GetAddress(2, 3, ExcelPackage.MaxRows, 3);

                var IsGoldListExcelDropDown = workSheet.DataValidations.AddListValidation(IsGoldRangeLookUp);
                IsGoldListExcelDropDown.Formula.ExcelFormula = "IsGold!" + IsGoldRange.ToString();


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

                            valid = decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString() ?? "", out weight);
                            itemObj.Weight = valid ? (decimal?)weight : null;
                            var isGoldValue = (worksheet.Cells[row, 3].Value?.ToString() ?? "").Trim().ToLower();
                            itemObj.IsGold = isGoldValue == "true";

                            valid = int.TryParse(worksheet.Cells[row, 5].Value?.ToString() ?? "", out quantity);
                            itemObj.quantity = valid ? quantity : 0;

                            valid = decimal.TryParse(worksheet.Cells[row, 6].Value?.ToString() ?? "", out cost);
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

        [CustomAuthorize(AccessName = "SalesOrdersDetails.CanAdd")]
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
                            obj.SalesType_id = item.SalesTypeId;
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
                                                                    , obj.location_id)
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