using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWC.IMS.Models
{
    [NotMapped]
    public partial class DemoSource
    {
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Required]
        [StringLength(255)]
        public string BRReferenceNo { get; set; }

        [Required]
        [StringLength(255)]
        public string ToName { get; set; }

        [Required]
        [StringLength(255)]
        public string FromName { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(255)]
        public string BusinessUnitCode { get; set; }

        [Required]
        [StringLength(255)]
        public string BusinessUnitName { get; set; }

        [Column(TypeName = "date")]
        public DateTime DurationFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DurationTo { get; set; }

        [Required]
        [StringLength(255)]
        public string BillingAddressedTo { get; set; }

        [StringLength(255)]
        public string Thru { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? LockDatetime { get; set; }

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

        public Guid UploadBatchNumber { get; set; }

        public int? BillingRequestHeader_WorkflowAction { get; set; }

        [StringLength(255)]
        public string Remarks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DemoSourceDetail> DemoSourceDetails { get; set; }

       
    }
}
