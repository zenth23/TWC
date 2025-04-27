using System;
using System.ComponentModel.DataAnnotations;
using TWC.IMS.Common.HelperClasses;


namespace TWC.IMS.Web.Models
{
    public class ProductInventoryViewModel
    {

        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTimeOffset? Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }

        //public string LocationName { get; set; }
        //public string SalesTypeName { get; set; }
        //public string ProductName { get; set; }
        //public int Quantity { get; set; }
    }
}