namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SalesType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesType()
        {
            //Product_Inventory = new HashSet<Product_Inventory>();
            //InventoryEntries = new HashSet<Inventory_Entry>();
            SalesOrderHeaders = new HashSet<SalesOrderHeader>();
        }
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }
        [DisplayName("Sales Type")]

        [StringLength(100)]
        public string SalesType_name { get; set; }

        [DisplayName("Description")]
        [StringLength(255)]
        public string Description { get; set; }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Product_Inventory> Product_Inventory { get; set; }
        //public virtual ICollection<Inventory_Entry> InventoryEntries { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
    }
}
