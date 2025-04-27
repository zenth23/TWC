using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWC.IMS.Models;

namespace TWC.IMS.Web.Models
{
    public class DashboardViewModel
    {
        public decimal TotalInventoryValue { get; set; }
        public int TotalItems { get; set; }
        public List<SalesOrderHeader> RecentTransactions { get; set; }
        public List<Product_Inventory> LowStockItems { get; set; }

    }


    public class TransactionItem
    {
        public DateTime Created { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
    }
}