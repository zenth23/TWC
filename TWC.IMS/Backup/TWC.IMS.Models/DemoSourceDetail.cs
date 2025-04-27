using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWC.IMS.Models
{ 
    [NotMapped]
    public partial class DemoSourceDetail
    {
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Required]
        [StringLength(255)]
        public string WbsCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime Duration { get; set; }

        [Required]
        [StringLength(255)]
        public string Particular { get; set; }

        [StringLength(255)]
        public string LicenseCode { get; set; }

        [StringLength(255)]
        public string Product { get; set; }

        public int Qty { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IncludeInSum { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(255)]
        public string LicenseCurrencyCode { get; set; }

        [Required]
        [StringLength(255)]
        public string WbsSubscriptionTypeCode { get; set; }

        [Required]
        [StringLength(255)]
        public string LicenseSubscriptionTypeCode { get; set; }

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

        public int DemoSourceDetail_DemoSource { get; set; }

        public virtual DemoSource DemoSource { get; set; }
    }
}
