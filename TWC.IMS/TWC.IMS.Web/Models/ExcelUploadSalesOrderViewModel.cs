using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class ExcelUploadSalesOrderViewModel
    {
        public string ProductName { get; set; }
        public string InvoiceNumber { get; set; }
        public int LocationId { get; set; }
        public int SalesTypeId { get; set; }
        public decimal? Weight { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public bool IsGold { get; set; }
    }
}