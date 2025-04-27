using System;
using System.ComponentModel.DataAnnotations;
using TWC.IMS.Common.HelperClasses;


namespace TWC.IMS.Web.Models
{
    public class ProductsViewModel
    {

        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Display(Name = "Product")]
        [StringLength(100)]
        public string product_name { get; set; }

        [Display(Name = "Type")]
        public int product_type { get; set; }

        [Display(Name = "Karat")]
        public int? karat { get; set; }

        //[Display(Name = "Weight")]
        //public decimal? weight { get; set; }

        [Display(Name = "Material")]
        [StringLength(50)]
        public string material { get; set; }

        [Display(Name = "Gemstone")]
        [StringLength(255)]
        public string gemstones { get; set; }

        [Display(Name = "Retail Price")]
        public decimal? retail_price { get; set; }
        [Display(Name = "Selling Price")]
        public decimal? selling_price { get; set; }
        public int LowStockThreshold { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTimeOffset? Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }

        //public virtual Location Location { get; set; }

        //public virtual Product_Master Product_Master { get; set; }

        //public virtual SalesType SalesType { get; set; }



        [Display(Name = "Type")]
        public string ProductTypeDescription { get; set; }
    }
}