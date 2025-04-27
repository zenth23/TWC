using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class InventoryEntryViewModel : TWC.IMS.Models.Inventory_Entry
    {
        
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public string LocationName { get; set; }
        public string CategoryName { get; set; }

        public string ValidationMessage { get; set; }
    }
}