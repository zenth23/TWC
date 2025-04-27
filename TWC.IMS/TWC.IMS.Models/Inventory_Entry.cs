namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Inventory_Entry
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        public int? inventory_id { get; set; }

        [StringLength(50)]
        public string batch_number { get; set; }

        public int? quantity { get; set; }

        public DateTime? entry_date { get; set; }

        public DateTime? received_date { get; set; }

        //[StringLength(50)]
        //public string entry_type { get; set; }

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

        public int location_id { get; set; }
       
        public int category_id { get; set; }
        public int product_id { get; set; }

        public bool deleted { get; set; }
        public virtual Product_Inventory Product_Inventory { get; set; }

        public virtual Location Location { get; set; }
        public virtual Category Category { get; set; }
        
        public virtual Product_Master Product_Master { get; set; }
    }
}
