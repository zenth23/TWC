using TWC.IMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    public class UserViewModel
    {
        public UserViewModel()
        {
            this.AccountModel = new AccountViewModel();
            this.ChangePasswordModel = new ChangePasswordViewModel();
            this.ContactModel = new ContactViewModel();
            this.PersonalModel = new PersonalViewModel();
        }

        public int Id { get; set; }

        public Guid UniqueKey { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? Created { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? Modified { get; set; }

        // for Kendo Combobox (data entry). proxy for AccountModel.User_Role only
        // kendo does not support greater than 1 underscores in an element name
        [Required]
        [Display(Name = "Role")]
        public string User_Role { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        [Display(Name = "Activation Date")]
        public DateTime? ActivationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        [Display(Name = "Deactivation Date")]
        public DateTime? DeactivationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        [Display(Name = "Last Login Date")]
        public DateTime? LastLoginDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        [Display(Name = "Account is Valid Until")]
        public DateTime? ExpirationDate { get; set; }

        public ChangePasswordViewModel ChangePasswordModel { get; set; }

        public PersonalViewModel PersonalModel { get; set; }

        public AccountViewModel AccountModel { get; set; }

        public ContactViewModel ContactModel { get; set; }
    }

    [Serializable]
    public class PersonalViewModel
    {
        /// <summary>
        /// For UserDetail.Id use only
        /// </summary>
        public int Id { get; set; }

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

        [Display(Name = "Name")]
        public string FullName { get; set; }

        public string Suffix { get; set; }

        [StringLength(255)]
        public string Nickname { get; set; }

        public byte[] Avatar { get; set; }

        [Display(Name = "Username")]
        [StringLength(128)]
        public string UserDetail_AspNetUser { get; set; }

        [Timestamp]
        public byte[] UserDetailRowVersion { get; set; }
    }

    [Serializable]
    public class AccountViewModel
    {
        public AccountViewModel()
        {
            this.ChangePasswordModel = new ChangePasswordViewModel();
            this.AccountSetting = new IndexViewModel();
        }

        public string UserId { get; set; }

        public Guid UniqueKey { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        // not required
        // for viewing only
        [Display(Name = "Role")]
        public string User_Role { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Display(Name = "Administrator")]
        public bool IsAdmin { get; set; }

        public string Status { get; set; }

        public DateTime? LockoutEndDate { get; set; }

        //[DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        //[Display(Name = "Account is Valid Until")]
        //public DateTime? ExpirationDate { get; set; }

        public ChangePasswordViewModel ChangePasswordModel { get; set; }

        public IndexViewModel AccountSetting { get; set; }
    }

    [Serializable]
    public class ContactViewModel
    {
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsEmailVerified { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^(09)\\d{9}$")] //09XXXXXXXXX, 11digits
        public string PhoneNumber { get; set; }

        public bool IsPhoneVerified { get; set; }
    }
}