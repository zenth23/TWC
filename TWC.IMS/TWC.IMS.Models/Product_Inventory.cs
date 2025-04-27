namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product_Inventory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_Inventory()
        {
            Asset_Movement = new HashSet<Asset_Movement>();
            Inventory_Entry = new HashSet<Inventory_Entry>();
        }
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        public int? product_id { get; set; }

        public int? location_id { get; set; }


        public int? quantity { get; set; }

        [StringLength(50)]
        public string status { get; set; }

        [StringLength(50)]
        public string ownership { get; set; }

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
        public virtual ICollection<Asset_Movement> Asset_Movement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inventory_Entry> Inventory_Entry { get; set; }

        public virtual Location Location { get; set; }

        public virtual Product_Master Product_Master { get; set; }
        
    }
}
