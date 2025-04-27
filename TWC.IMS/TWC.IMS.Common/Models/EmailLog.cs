namespace TWC.IMS.Common.Models
{
    using HelperClasses;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Text;

    [Serializable]
    [Table("EmailLogs")]
    public partial class EmailLog
    {
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
        
        [StringLength(255)]
        public string Status { get; set; }

        public DateTime? ResentDatetime { get; set; }

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
