// REQUEST FOR REMOVAL

using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Models.HelperClasses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWC.IMS.Models
{
    [Serializable]
    [Table("Requests")]
    [Request(TableName = "Requests", 
        StatusColumn = "Request_Status", 
        TransactionNoColumn = "Name", 
        Url = "Requests/Details/{UniqueKey}", 
        UniqueKeyColumn = "UniqueKey",
        ProponentColumn = "Request_Proponent")]
    public partial class Request : DescribableEntity
    {
        [Key]
        public int Id { get; set; }
        public Guid UniqueKey { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Request_Status { get; set; }
        public int Request_Proponent { get; set; }

        [Display(Name = "Created By")]
        [StringLength(255)]
        [Required]
        public string CreatedBy { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset Created { get; set; }

        [Display(Name = "Modified By")]
        [StringLength(255)]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Modified { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual StatusSet Status { get; set; }
        public virtual UserDetail Proponent { get; set; }
    }
}
