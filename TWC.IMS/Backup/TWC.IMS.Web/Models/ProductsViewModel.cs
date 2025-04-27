using System;
using System.ComponentModel.DataAnnotations;
using TWC.IMS.Common.HelperClasses;


namespace TWC.IMS.Web.Models
{
    public class ProductsViewModel
    {

        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [StringLength(100)]
        public string product_name { get; set; }

        [StringLength(50)]
        public string product_type { get; set; }

        public int? karat { get; set; }

        public decimal? weight { get; set; }

        [StringLength(50)]
        public string material { get; set; }

        [StringLength(255)]
        public string gemstones { get; set; }

        public decimal? retail_price { get; set; }

        public decimal? selling_price { get; set; }


        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTimeOffset? Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }

        //public virtual Location Location { get; set; }

        //public virtual Product_Master Product_Master { get; set; }

        //public virtual Supplier Supplier { get; set; }
    }
}