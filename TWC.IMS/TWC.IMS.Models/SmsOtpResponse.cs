namespace TWC.IMS.Models
{
    using Common.HelperClasses;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Serializable]
    [Table("SmsOtpResponses")]
    public partial class SmsOtpResponse: DescribableEntity
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Required]
        [StringLength(255)]
        public string Status { get; set; }
        
        [StringLength(255)]
        public string Sid { get; set; }

        public DateTime? ValidUntil { get; set; }

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
    }
}
