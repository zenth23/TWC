namespace TWC.IMS.Models
{
    using TWC.IMS.Common.HelperClasses;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("ErrorLogs")]
    public partial class ErrorLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Unique Key")]
        public Guid UniqueKey { get; set; }

        [Display(Name = "Error Number")]
        public long ErrorNumber { get; set; }

        [Required]
        [Display(Name = "Error Message")]
        public string ErrorMessage { get; set; }

        [Required]
        [Display(Name = "Message Type")]
        [StringLength(50)]
        public string MessageType { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Method Name")]
        public string MethodName { get; set; }

        [Display(Name = "Friendly Error Message")]
        public string FriendlyErrorMessage { get; set; }

        [Display(Name = "Exception")]
        public string Exception { get; set; }

        [Display(Name = "Param Data")]
        public string ParamData { get; set; }

        [Display(Name = "App Version")]
        public string AppVersion { get; set; }

        [Display(Name = "User Role")]
        public string UserRole { get; set; }

        [Display(Name = "Is Mobile Device")]
        public bool? IsMobileDevice { get; set; }

        [Display(Name = "Environment")]
        public string Environment { get; set; }

        [Display(Name = "Impact Level")]
        public string ImpactLevel { get; set; }

        public string ClientIPAddress { get; set; }

        public string UserAgent { get; set; }

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
    }
}
