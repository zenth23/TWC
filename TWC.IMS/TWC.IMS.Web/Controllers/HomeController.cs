using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TWC.IMS.Models;
using TWC.IMS.BL; // Ensure this namespace includes your business logic classes
using TWC.IMS.Web.HelperClasses;
using System.Web;
using System.Globalization;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class HomeController : BaseController
    {



        [Authorize]
        public async Task<ActionResult> Index()
        {
            ViewBag.FirstName = await User.Identity.GetFirstNameAsync().ConfigureAwait(false);
            await ReportHelpers.GetReportExpirationAsync(this.HttpContext).ConfigureAwait(false);

            return View();
        }

        [AllowAnonymous]
        public ActionResult PrivacyStatement()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CreatePSCookie()
        {
            HttpCookie psCookie = HttpContext.Response.Cookies["privacyCookie"] ?? new HttpCookie("privacyCookie");
            psCookie.Value = "AGREED";
            psCookie.Expires = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddDays(1);
            //Response.SetCookie(psCookie);
            this.ControllerContext.HttpContext.Response.Cookies.Add(psCookie);

            return Json(new { Agreed = true }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetPSCookie()
        {
            bool agreed = false;
            HttpCookie psCookie = Request.Cookies["privacyCookie"];
            if (psCookie != null && psCookie.Value == "AGREED")
                agreed = true;

            return Json(new { Agreed = agreed }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult CookiePolicy()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CreateCNCookie()
        {
            HttpCookie cnCookie = HttpContext.Response.Cookies["cookieNoticeCookie"] ?? new HttpCookie("cookieNoticeCookie");
            cnCookie.Value = "AGREED";
            cnCookie.Expires = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddDays(1);
            //Response.SetCookie(psCookie);
            this.ControllerContext.HttpContext.Response.Cookies.Add(cnCookie);

            return Json(new { Agreed = true }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetCNCookie()
        {
            bool agreed = false;
            HttpCookie cnCookie = Request.Cookies["cookieNoticeCookie"];
            if (cnCookie != null && cnCookie.Value == "AGREED")
                agreed = true;

            return Json(new { Agreed = agreed }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        [SkipLogActionFilter]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult
            {
                Data = "Beat generated"
            };
        }

        public ActionResult VersionHistory()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetTotalInventoryData()
        {
            try
            {
                using (var inventoryBL = new BL.Product_Inventory(User.Identity.Name))
                {
                    var data = await inventoryBL.GetListAsync("Product_Master").ConfigureAwait(false);
                    var totalValue = data.Sum(x => x.Product_Master.selling_price * (x.quantity ?? 0)); // Assuming Product_Master.Price holds item price
                    var totalItems = data.Count();
                    var lowStockCount = data.Count(x => (x.quantity ?? 0) <= x.Product_Master.LowStockThreshold); // Assuming LowStockThreshold is part of Product_Master

                    return Json(new
                    {
                        success = true,
                        totalValue,
                        totalItems,
                        lowStockCount
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetHighValueItems()
        {
            try
            {
                using (var inventoryBL = new BL.Product_Inventory(User.Identity.Name))
                {
                    var data = await inventoryBL.GetListAsync("Product_Master").ConfigureAwait(false);

                    // Define high value items as those with a price above a certain threshold (e.g., 10,000)
                    var highValueItems = data
                        .Where(x => x.Product_Master.selling_price > 10000) // Adjust threshold as needed
                        .Select(x => new
                        {
                            ProductName = x.Product_Master.product_name, // Assuming Product_Master.Name holds the product name
                            Price = x.Product_Master.selling_price,
                            Quantity = x.quantity ?? 0
                        })
                        .OrderByDescending(x => x.Price)
                        .Take(5) // Top 5 high value items
                        .ToList();

                    return Json(new { success = true, highValueItems }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetLowStockAlerts()
        {
            try
            {
                using (var inventoryBL = new BL.Product_Inventory(User.Identity.Name))
                {
                    var data = await inventoryBL.GetListAsync("Product_Master").ConfigureAwait(false);

                    // Define low stock items as those with quantity below a certain threshold (e.g., 10)
                    var lowStockItems = data
                        .Where(x => x.quantity <= x.Product_Master.LowStockThreshold) // Assuming Product_Master.LowStockThreshold exists
                        .Select(x => new
                        {
                            ProductName = x.Product_Master.product_name, // Assuming Product_Master.Name holds the product name
                            Price = x.Product_Master.selling_price,
                            Quantity = x.quantity ?? 0
                        })
                        .OrderBy(x => x.Quantity)
                        .Take(5) // Top 5 low stock items
                        .ToList();

                    return Json(new { success = true, lowStockItems }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetSalesOrderVsInventoryData()
        {
            try
            {
                using (var salesOrderBL = new SalesOrders(User.Identity.Name))
                using (var inventoryBL = new BL.Product_Inventory(User.Identity.Name))
                {
                    var salesOrders = await salesOrderBL.GetListAsync().ConfigureAwait(false);
                    var inventoryEntries = await inventoryBL.GetListAsync().ConfigureAwait(false);

                    var data = new
                    {
                        SalesOrders = salesOrders.Count(),
                        InventoryEntries = inventoryEntries.Count()
                    };

                    return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public async Task<JsonResult> GetRecentTransactions()
        {
            try
            {
                using (var salesOrdersBL = new SalesOrders(User.Identity.Name))
                {
                    var data = await salesOrdersBL.GetListAsync("SalesOrderDetails", "SalesOrderDetails.Product_Master").ConfigureAwait(false);
                    var transactions = data
                        .Where(x => !x.IsDeleted) // Exclude deleted sales orders
                        .OrderByDescending(x => x.Created)
                        .Take(5)
                        .SelectMany(header => header.SalesOrderDetails.Select(detail => new
                        {
                            Date = header.Created.Value.DateTime.ToString(),
                            InvoiceNumber = header.InvoiceNumber,
                            ProductName = detail.Product_Master.product_name, // Assuming Product_Master.Name holds item name
                            Quantity = detail.Qty,
                            Cost = detail.Cost,
                            Total = detail.isGold  ? detail.Weight * detail.Cost : detail.Qty * detail.Cost
                        }))
                        .ToList();

                    return Json(new { success = true, transactions }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //public async Task<JsonResult> GetSalesCountByFilter(string filterType)
        //{
        //    try
        //    {
        //        using (var salesOrdersBL = new SalesOrders(User.Identity.Name))
        //        {
        //            DateTime startDate = DateTime.Now;

        //            // Set the start date based on the filter type
        //            if (filterType.ToLower() == "day")
        //            {
        //                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Start from the 1st of the current month
        //            }
        //            else if (filterType.ToLower() == "week")
        //            {
        //                startDate = DateTime.Today.AddDays(-7);
        //            }
        //            else if (filterType.ToLower() == "month")
        //            {
        //                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Start from the 1st of the current month
        //            }
        //            else
        //            {
        //                throw new ArgumentException($"Invalid filter type: {filterType}", nameof(filterType));
        //            }

        //            // Get the list of sales orders with their details and associated product master data
        //            var salesOrders = await salesOrdersBL
        //                .GetListAsync("SalesOrderDetails", "SalesOrderDetails.Product_Master")
        //                .ConfigureAwait(false);

        //            // Group and filter the data based on the selected filter type
        //            var filteredData = salesOrders
        //                .SelectMany(s => s.SalesOrderDetails)
        //                .Where(d => d.SalesOrderHeader.Created >= startDate) // Filter based on the start date
        //                .Select(d => new
        //                {
        //                    GroupKey = d.SalesOrderHeader.Created.Value.Day.ToString("D2"), // Group by day of the month
        //            d.Cost,
        //                    d.Qty,
        //                    d.SalesOrderHeader.Created
        //                })
        //                .GroupBy(x => x.GroupKey)
        //                .Select(g => new
        //                {
        //                    Label = g.Key,
        //                    TotalSales = g.Sum(d => d.Cost),
        //                    TotalCount = g.Sum(d => d.Qty)
        //                })
        //                .OrderBy(g => g.Label) // Ensure days are in order
        //                .ToList();

        //            // Create the full list for the entire month (1st to 31st) to ensure all days are shown
        //            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        //            var fullMonthData = daysInMonth.Select(day => new
        //            {
        //                Label = day.ToString("D2"),
        //                TotalSales = filteredData.FirstOrDefault(d => d.Label == day.ToString("D2"))?.TotalSales ?? 0,
        //                TotalCount = filteredData.FirstOrDefault(d => d.Label == day.ToString("D2"))?.TotalCount ?? 0
        //            }).ToList();

        //            // Pass the current month as well
        //            string currentMonth = DateTime.Now.ToString("MMMM yyyy");

        //            return Json(new { success = true, data = fullMonthData, month = currentMonth }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
     

        public async Task<JsonResult> GetSalesCountByFilter(string filterType)
        {
            try
            {
                if (string.IsNullOrEmpty(filterType))
                {
                    return Json(new { success = false, message = "filterType is null or empty" }, JsonRequestBehavior.AllowGet);
                }

                // Debug log to confirm filterType
                Console.WriteLine($"Filter Type: {filterType}");

                using (var salesOrdersBL = new SalesOrders(User.Identity.Name))
                {
                    DateTime startDate = DateTime.Now;
                    DateTime endDate = DateTime.Now;

                    // Set the date range based on filterType
                    if (filterType.ToLower() == "today")
                    {
                        startDate = DateTime.Today;
                        endDate = DateTime.Today.AddDays(1); // Next day to cover the whole day
                    }
                    else if (filterType.ToLower() == "thisweek")
                    {
                        startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek); // Start of the week (Sunday)
                        endDate = startDate.AddDays(7); // End of the week
                    }
                    else if (filterType.ToLower() == "lastweek")
                    {
                        startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7); // Last week's start
                        endDate = startDate.AddDays(7); // Last week's end
                    }
                    else if (filterType.ToLower() == "thismonth")
                    {
                        startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // First day of this month
                        endDate = startDate.AddMonths(1); // End of this month
                    }
                    else if (filterType.ToLower() == "lastmonth")
                    {
                        startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1); // First day of last month
                        endDate = startDate.AddMonths(1); // End of last month
                    }
                    else if (filterType.ToLower() == "thisyear")
                    {
                        startDate = new DateTime(DateTime.Now.Year, 1, 1); // First day of this year
                        endDate = startDate.AddYears(1); // End of this year
                    }
                    else if (filterType.ToLower() == "lastyear")
                    {
                        startDate = new DateTime(DateTime.Now.Year - 1, 1, 1); // First day of last year
                        endDate = startDate.AddYears(1); // End of last year
                    }
                    else if (filterType.ToLower() == "day")
                    {
                        startDate = DateTime.Today;
                        endDate = DateTime.Today;
                    }
                    else if (filterType.ToLower() == "week")
                    {
                        startDate = DateTime.Today.AddDays(-7);
                        endDate = DateTime.Today;
                    }
                    else if (filterType.ToLower() == "month" || filterType.ToLower() == "category")
                    {
                        startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        endDate = DateTime.Now;
                    }
                    else
                    {
                        return Json(new { success = false, message = $"Invalid filter type: {filterType}" }, JsonRequestBehavior.AllowGet);
                    }

                    // Convert the startDate and endDate to DateTimeOffset
                    DateTimeOffset startDateOffset = new DateTimeOffset(startDate);
                    DateTimeOffset endDateOffset = new DateTimeOffset(endDate);

                    // Get sales orders with product details
                    var salesOrders = await salesOrdersBL
                       .GetListAsync("SalesOrderDetails", "SalesOrderDetails.Product_Master", "SalesOrderDetails.Product_Master.Product_Type")
                       .ConfigureAwait(false);

                    // Filter and group the data by day and category (Product Type)
                    var filteredData = salesOrders
                        .SelectMany(s => s.SalesOrderDetails) // Flatten the sales order details
                        .Where(d => d.SalesOrderHeader.Created >= startDateOffset && d.SalesOrderHeader.Created <= endDateOffset) // Filter by date range
                        .GroupBy(d => new
                        {
                            Day = d.SalesOrderHeader.Created.Value.Date, // Group by the day (ignore time)
                    Category = d.Product_Master.Product_Type.Description // Group by category
                })
                        .Select(g => new
                        {
                            Day = g.Key.Day.ToString("yyyy-MM-dd"), // Format the day
<<<<<<< HEAD
                            Category = g.Key.Category,

                            TotalSales = g.Sum(d => d.isGold? d.Weight * d.Cost : d.Qty * d.Cost),

                          //  TotalSales = g.Sum(d => d.SalesOrderHeader.Amount), // change d.cost to d.salesorderheaderamount
=======
                    Category = g.Key.Category,
                            TotalSales = g.Sum(d => d.Cost),
<<<<<<< HEAD
>>>>>>> parent of 2d84c16 (Dashboard compute correction and Sales order upload function bug fix)
=======
>>>>>>> parent of 2d84c16 (Dashboard compute correction and Sales order upload function bug fix)
                            TotalCount = g.Sum(d => d.Qty)
                        })
                        .OrderBy(d => d.Day) // Ensure the days are in order
                        .ToList();

                    // Group by the day for each category
                    var groupedData = filteredData
                        .GroupBy(d => d.Day)
                        .Select(g => new
                        {
                            Day = g.Key,
                            Categories = g.ToList()
                        })
                        .ToList();

                    return Json(new { success = true, data = groupedData }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetGroupKey(string filterType, SalesOrderDetail d)
        {
            if (filterType == "day")
            {
                return d.SalesOrderHeader.Created.Value.DateTime.ToString("yyyy-MM-dd"); // Convert DateTimeOffset to DateTime
            }
            else if (filterType == "week")
            {
                return GetWeekOfYear(d.SalesOrderHeader.Created.Value.DateTime); // Convert DateTimeOffset to DateTime
            }
            else if (filterType == "month")
            {
                return d.SalesOrderHeader.Created.Value.DateTime.ToString("yyyy-MM"); // Convert DateTimeOffset to DateTime
            }
            else
            {

                return d.Product_Master.Product_Type.Description;

            }
        }

        // Custom method to get the week number in a way compatible with .NET Framework 4.7.2
        private string GetWeekOfYear(DateTime date)
        {
            var cal = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            int weekNo = cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNo.ToString();
        }



        //public async Task<JsonResult> GetSalesCountByCategory(string filterType)
        //{
        //    try
        //    {
        //        using (var salesOrdersBL = new SalesOrders(User.Identity.Name))
        //        {
        //            DateTime startDate = DateTime.Now;

        //            switch (filterType)
        //            {
        //                case "Day":
        //                    startDate = DateTime.Today;
        //                    break;
        //                case "Week":
        //                    startDate = DateTime.Today.AddDays(-7);
        //                    break;
        //                case "Month":
        //                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //                    break;
        //            }

        //            var salesOrders = await salesOrdersBL
        //    .GetListAsync("SalesOrderDetails", "SalesOrderDetails.Product_Master")
        //    .ConfigureAwait(false);

        //            var filteredData = salesOrders
        //                .Where(s => s.SalesOrderDetails.Any(d => d.SalesOrderHeader.Created >= startDate))
        //                .SelectMany(s => s.SalesOrderDetails)
        //                .GroupBy(d => d.Product_Master.Product_Type)
        //                .Select(g => new
        //                {
        //                    Category = g.Key,
        //                    TotalSales = g.Sum(d => d.Cost),
        //                    TotalCount = g.Sum(d => d.Qty)
        //                })
        //                .OrderByDescending(s => s.TotalSales)
        //                .ToList();

        //            return Json(new { success = true, data = filteredData }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        [HttpPost]
        public async Task<JsonResult> GetInvestmentOverview()
        {
            try
            {
                using (var inventoryBL = new BL.Product_Inventory(User.Identity.Name))
                {
                    var data = await inventoryBL.GetListAsync("Product_Master").ConfigureAwait(false);
                    var investmentData = data
                        .GroupBy(x => x.Product_Master.Product_Type) // Assuming Product_Master.Category is a category field
                        .Select(g => new
                        {
                            Category = g.Key,
                            TotalValue = g.Sum(x => x.Product_Master.selling_price * (x.quantity ?? 0))
                        });

                    return Json(new { success = true, investmentData }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<JsonResult> GetTotalSalesByFilter(string filterType)
        {
            try
            {
                using (var salesOrdersBL = new SalesOrders(User.Identity.Name))
                {
                    DateTime startDate = DateTime.Now;
                    DateTime endDate = DateTime.Now;

                    // Determine the start and end dates based on the filterType
                    if (filterType.ToLower() == "today")
                    {
                        startDate = DateTime.Today;
                        endDate = DateTime.Today.AddDays(1); // Next day to cover the whole day
                    }
                    else if (filterType.ToLower() == "thisweek")
                    {
                        startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek); // Start of the week (Sunday)
                        endDate = startDate.AddDays(7); // End of the week
                    }
                    else if (filterType.ToLower() == "lastweek")
                    {
                        startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7); // Last week's start
                        endDate = startDate.AddDays(7); // Last week's end
                    }
                    else if (filterType.ToLower() == "thismonth")
                    {
                        startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // First day of this month
                        endDate = startDate.AddMonths(1); // End of this month
                    }
                    else if (filterType.ToLower() == "lastmonth")
                    {
                        startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1); // First day of last month
                        endDate = startDate.AddMonths(1); // End of last month
                    }
                    else if (filterType.ToLower() == "thisyear")
                    {
                        startDate = new DateTime(DateTime.Now.Year, 1, 1); // First day of this year
                        endDate = startDate.AddYears(1); // End of this year
                    }
                    else if (filterType.ToLower() == "lastyear")
                    {
                        startDate = new DateTime(DateTime.Now.Year - 1, 1, 1); // First day of last year
                        endDate = startDate.AddYears(1); // End of last year
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid filter type: {filterType}", nameof(filterType));
                    }

                    // Get the sales orders within the date range
                    var salesOrders = await salesOrdersBL
                        .GetListAsync("SalesOrderDetails", "SalesOrderDetails.Product_Master")
                        .ConfigureAwait(false);

                    // Calculate the total sales for the filtered data
                    var totalSales = salesOrders
                        .SelectMany(s => s.SalesOrderDetails)
                        .Where(d => d.SalesOrderHeader.Created >= startDate && d.SalesOrderHeader.Created < endDate)
                        .Sum(d => d.Cost); // Sum of sales cost

                    return Json(new { success = true, totalSales = totalSales }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
