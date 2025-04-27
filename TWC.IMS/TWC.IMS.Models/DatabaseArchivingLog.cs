namespace TWC.IMS.Models
{
    using TWC.IMS.Common.HelperClasses;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("DatabaseArchivingLogs")]
    public partial class DatabaseArchivingLog
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Display(Name = "Log")]
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Display(Name = "Message")]
        public string Description { get; set; }

        [Display(Name = "Archived By")]
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
