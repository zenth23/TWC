namespace TWC.IMS.Models
{
    using TWC.IMS.Common.HelperClasses;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("AuditLogs")]
    public partial class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Display(Name = "Table Name")]
        [Required]
        [StringLength(256)]
        public string TableName { get; set; }

        [Display(Name = "Event Type")]
        [StringLength(50)]
        public string EventType { get; set; }

        [Display(Name = "Row ID")]
        [StringLength(256)]
        public string RowID { get; set; }

        [Display(Name = "Column Name")]
        [StringLength(256)]
        public string ColumnName { get; set; }

        [Display(Name = "Old Value")]
        public string OldValue { get; set; }

        [Display(Name = "New Value")]
        public string NewValue { get; set; }

        [Display(Name = "Timestamp")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Created { get; set; }

        [Display(Name = "Username")]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
