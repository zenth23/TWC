using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class ExcelUploadInventoryEntryViewModel
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int LocationId { get; set; }
        //public int SalesTypeId { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }
    }
}