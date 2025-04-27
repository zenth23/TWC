namespace TWC.IMS.Models
{
    using TWC.IMS.Common.HelperClasses;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("PasswordHistories")]
    public partial class PasswordHistory : DescribableEntity
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsTemporaryPassword { get; set; }

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

        [Required]
        [StringLength(128)]
        public string PasswordHistory_AspNetUser { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
