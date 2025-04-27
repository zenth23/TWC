using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class ProductInventoryListViewModel
    {
        public int Id { get; set; }
        public Guid UniqueKey { get; set; }
        public int? ImageId { get; set; }
        public string ProductName { get; set; }
        public int? Inventory { get; set; }
        //public int? SalesTypeId { get; set; }
        public int? LocationId { get; set; }
        //public string SalesTypeName { get; set; }
        public string LocationName { get; set; }
        public string Url { get; set; }
    }
}