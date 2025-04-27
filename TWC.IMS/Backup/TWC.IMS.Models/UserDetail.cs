namespace TWC.IMS.Models
{
    using TWC.IMS.Common.HelperClasses;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("UserDetails")]
    public partial class UserDetail : DescribableEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserDetail()
        {
 
            SystemNotifications = new HashSet<SystemNotification>();
            Requests = new HashSet<Request>();
        }

        [Key]
        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [StringLength(255)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(255)]
        public string Suffix { get; set; }

        [StringLength(255)]
        public string Nickname { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [Display(Name = "Last Login Date")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? LastLoginDatetime { get; set; }

        [Display(Name = "Activation Date")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? ActivationDatetime { get; set; }

        [Display(Name = "Deactivation Date")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? DeactivationDatetime { get; set; }

        [Display(Name = "Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? ExpirationDatetime { get; set; }

        [Display(Name = "Avatar")]
        public byte[] Avatar { get; set; }

        // not to be display on UI
        [StringLength(50)]
        public string AvatarMimeType { get; set; }

        [StringLength(255)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Created { get; set; }

        [StringLength(255)]
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Modified { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Required]
        [StringLength(128)]
        public string UserDetail_AspNetUser { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<SystemNotification> SystemNotifications { get; set; }
        public virtual ICollection<Request> Requests { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                var middle = !string.IsNullOrWhiteSpace(this.MiddleName) ? " " + this.MiddleName[0].ToString().ToUpper() + ". " : string.Empty;
                return $"{this.LastName}, {this.FirstName}{middle}";
                //return $"{this.LastName}, {this.FirstName}";
            }
        }
    }
}
