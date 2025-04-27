namespace TWC.IMS.Models
{
    using TWC.IMS.Common.HelperClasses;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("RolePermissions")]
    public partial class RolePermission : DescribableEntity
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

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

        [Required]
        [StringLength(128)]
        public string RolePermission_Role { get; set; }

        public int RolePermission_ModuleAccess { get; set; }

        public virtual AspNetRole AspNetRole { get; set; }

        public virtual ModuleAccess ModuleAccess { get; set; }
    }
}
