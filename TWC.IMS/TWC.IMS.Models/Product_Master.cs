namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product_Master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_Master()
        {
            Product_Inventory = new HashSet<Product_Inventory>();
            Inventory_Entry = new HashSet<Inventory_Entry>();
            Product_Master_Images = new HashSet<Product_Master_Image>();
            SalesOrderDetails = new HashSet<SalesOrderDetail>();
        }

        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [StringLength(100)]
        public string product_name { get; set; }

        //[StringLength(50)]
        //public string product_type { get; set; }

        public int? karat { get; set; }

        //public decimal? weight { get; set; }

        [StringLength(50)]
        public string material { get; set; }

        [StringLength(255)]
        public string gemstones { get; set; }

        [DisplayName("Retail Price")]
        [Range(0, int.MaxValue, ErrorMessage = "Negative value is not alowed.")]
        public decimal? retail_price { get; set; }


        [DisplayName("Selling Price")]
        [Range(0, int.MaxValue, ErrorMessage = "Negative value is not alowed.")]
        public decimal? selling_price { get; set; }

        public decimal? LowStockThreshold { get; set; }
        public int ProductType_id { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTimeOffset Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ProductType Product_Type { get; set; }
        public virtual ICollection<Product_Inventory> Product_Inventory { get; set; }
        public virtual ICollection<Inventory_Entry> Inventory_Entry { get; set; }
        public virtual ICollection<Product_Master_Image> Product_Master_Images { get; set; }
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
    }
}
