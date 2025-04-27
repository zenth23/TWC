namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Asset_Movement
    {
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        public int? inventory_id { get; set; }

        public int? movement_type_id { get; set; }

        public int? shop_id { get; set; }

        public DateTime? movement_date { get; set; }

        public int? quantity { get; set; }

        [StringLength(255)]
        public string remarks { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTimeOffset? Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual Product_Inventory Product_Inventory { get; set; }

        public virtual Movement_Type Movement_Type { get; set; }

        public virtual Shop Shop { get; set; }
    }
}
