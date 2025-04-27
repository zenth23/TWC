namespace TWC.IMS.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Serializable]
    [Table("ErrorLogs")]
    public partial class ErrorLog
    {
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        public long ErrorNumber { get; set; }

        [Required]
        public string ErrorMessage { get; set; }

        [Required]
        [StringLength(50)]
        public string MessageType { get; set; }

        [Required]
        [StringLength(500)]
        public string MethodName { get; set; }

        public string FriendlyErrorMessage { get; set; }

        public string Exception { get; set; }

        public string ParamData { get; set; }

        public string AppVersion { get; set; }
        
        public string UserRole { get; set; }
        
        public bool? IsMobileDevice { get; set; }
        
        public string Environment { get; set; }
        
        public string ImpactLevel { get; set; }

        public string ClientIPAddress { get; set; }

        public string UserAgent { get; set; }

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
