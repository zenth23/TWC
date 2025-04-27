using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using OfficeOpenXml;
using TWC.IMS.BL; // Business logic layer
using TWC.IMS.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace TWC.IMS.Web.Controllers
{
    [Authorize]
    public class ReportGeneratorController : Controller
    {
        private readonly string _username;

        public ReportGeneratorController()
        {
            _username = User?.Identity?.Name ?? "System";
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Reports";
            return View();
        }

        // Low Stock Report
        [HttpPost]
        public async Task<ActionResult> DownloadLowStockReport()
        {
            try
            {
                using (var inventoryBL = new BL.Product_Inventory(_username))
                {
                    var data = await inventoryBL.GetListAsync("Product_Master").ConfigureAwait(false);

                    var lowStockItems = data
                        .Where(x => x.quantity <= x.Product_Master.LowStockThreshold)
                        .Select(x => new
                        {
                            ProductName = x.Product_Master.product_name,
                            Quantity = x.quantity ?? 0,
                            Price = x.Product_Master.selling_price,
                            Warehouse = x.Location?.location_name
                        })
                        .OrderBy(x => x.Quantity)
                        .ToList();

                    var fileContent = GenerateExcelReport(lowStockItems, "Low Stock Report");

                    // Returning the file as FileContentResult with appropriate MIME type
                    return new FileContentResult(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "LowStockReport.xlsx"
                    };
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Sales Order Report
        [HttpPost]
        public async Task<ActionResult> DownloadSalesOrderReport(string startDate, string endDate)
        {
            try
            {
                using (var salesOrdersBL = new SalesOrders(_username))
                {
                    var salesOrders = await salesOrdersBL.GetListAsync("SalesOrderDetails").ConfigureAwait(false);

                    var filteredOrders = salesOrders
                        .Where(x => x.Created >= DateTime.Parse(startDate) && x.Created <= DateTime.Parse(endDate))
                        .Select(x => new
                        {
                            OrderNumber = x.InvoiceNumber,
                            OrderDate = x.Created,
                            TotalAmount = x.Amount
                        })
                        .OrderBy(x => x.OrderDate)
                        .ToList();

                    var fileContent = GenerateExcelReport(filteredOrders, "Sales Order Report");

                    return new FileContentResult(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "SalesOrderReport.xlsx"
                    };
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // High Value Items Report
        [HttpPost]
        public async Task<ActionResult> DownloadHighValueItemsReport()
        {
            try
            {
                using (var inventoryBL = new BL.Product_Inventory(_username))
                {
                    var data = await inventoryBL.GetListAsync("Product_Master").ConfigureAwait(false);

                    var highValueItems = data
                        .OrderByDescending(x => x.Product_Master.selling_price)
                        .Take(10)
                        .Select(x => new
                        {
                            ProductName = x.Product_Master.product_name,
                            Price = x.Product_Master.selling_price,
                            Quantity = x.quantity ?? 0,
                            Warehouse = x.Location?.location_name
                        })
                        .ToList();

                    var fileContent = GenerateExcelReport(highValueItems, "High Value Items");

                    return new FileContentResult(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "HighValueItemsReport.xlsx"
                    };
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Product Inventory Report (JSON Format)
        public async Task<ActionResult> DownloadProductInventoryReport()
        {
            try
            {
                using (var inventoryBL = new BL.Product_Inventory(_username))
                {
                    var data = await inventoryBL.GetListAsync("Product_Master", "Location").ConfigureAwait(false);

                    var inventoryData = data
                        .Select(x => new
                        {
                            ProductName = x.Product_Master?.product_name,
                            Quantity = x.quantity ?? 0,
                            Price = x.Product_Master?.selling_price,
                            Location = x.Location?.location_name,
                            LastUpdated = x.Modified
                        })
                        .OrderBy(x => x.ProductName)
                        .ToList();
                

                    var fileContent = GenerateExcelReport(inventoryData, "Product Inventory");

                    return new FileContentResult(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "ProductInventoryReport.xlsx"
                    };
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private byte[] GenerateExcelReport<T>(List<T> data, string reportName)
        {

            using (var package = new ExcelPackage())
            {
                // Create a worksheet with the given report name
                var worksheet = package.Workbook.Worksheets.Add(reportName);
                var row = 1;

                // Get the properties of the generic type T (i.e., the column headers)
                var properties = typeof(T).GetProperties();

                // Add column headers to the first row
                foreach (var prop in properties)
                {
                    var cell = worksheet.Cells[row, properties.ToList().IndexOf(prop) + 1];
                    cell.Value = prop.Name;
                    // Make the header row bold
                    cell.Style.Font.Bold = true;
                }

                row++; // Move to the next row for the data

                // Add data rows to the worksheet
                foreach (var item in data)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cells[row, col + 1].Value = properties[col].GetValue(item);
                    }
                    row++;
                }

                // Auto-size the columns to fit content
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return the file as a byte array
                return package.GetAsByteArray();

            }

        }
    }
}
