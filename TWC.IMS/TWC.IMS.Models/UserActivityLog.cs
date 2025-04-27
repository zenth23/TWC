namespace TWC.IMS.Models
{
    using TWC.IMS.Common.HelperClasses;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("UserActivityLogs")]
    public partial class UserActivityLog
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Display(Name = "Activity")]
        [Required]
        [StringLength(500)]
        public string Activity { get; set; }

        [Display(Name = "Method Type")]
        [Required]
        [StringLength(50)]
        public string MethodType { get; set; }

        [Display(Name = "URL")]
        [Required]
        [StringLength(500)]
        public string AbsoluteUrl { get; set; }

        [Display(Name = "User Agent")]
        [StringLength(255)]
        public string UserAgent { get; set; }

        [Display(Name = "IP Address")]
        [StringLength(20)]
        public string ClientIPAddress { get; set; }

        [Display(Name = "Username")]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        [Display(Name = "Timestamp")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Modified { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Display(Name = "AppVersion")]
        public string AppVersion { get; set; }

        [Display(Name = "Is Mobile Device")]
        public bool? IsMobileDevice { get; set; }

        [Display(Name = "Session Id")]
        public string SessionId { get; set; }

        [Display(Name = "Session Start")]
        public DateTime? SessionStart { get; set; }
        
        [Display(Name = "Session Timeout (m)")]
        public int? SessionTimeout { get; set; }

        [Display(Name = "Role")]
        public string UserRole { get; set; }

        [Display(Name = "Form Data")]
        public string FormData { get; set; }
    }
}
