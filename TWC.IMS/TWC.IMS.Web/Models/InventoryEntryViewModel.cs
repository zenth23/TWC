using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Models
{
    public class InventoryEntryViewModel : TWC.IMS.Models.Inventory_Entry
    {
        
        public string ProductName { get; set; }
        //public string SalesTypeName { get; set; }
        public string LocationName { get; set; }
        public string CategoryName { get; set; }

        [AllowHtml]
        public string ValidationMessage { get; set; }
    }
}