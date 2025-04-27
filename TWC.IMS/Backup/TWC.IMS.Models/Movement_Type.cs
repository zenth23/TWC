namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Movement_Type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Movement_Type()
        {
            Asset_Movement = new HashSet<Asset_Movement>();
        }

        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Column("movement_type")]
        [StringLength(50)]
        public string movement_type1 { get; set; }

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
        public virtual ICollection<Asset_Movement> Asset_Movement { get; set; }
    }
}
