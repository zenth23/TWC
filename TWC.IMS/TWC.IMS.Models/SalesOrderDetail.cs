namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SalesOrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int Qty { get; set; }

        [DisplayName("Cost per Gram")]
        public decimal Cost { get; set; }

        public decimal? Weight { get; set; }

        public bool isGold { get; set; }
        public int SalesOrderDetail_Product { get; set; }

        public int SalesOrderDetail_SalesOrderHeader { get; set; }

        [Required]
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

        public virtual Product_Master Product_Master { get; set; }

        public virtual SalesOrderHeader SalesOrderHeader { get; set; }
    }
}
