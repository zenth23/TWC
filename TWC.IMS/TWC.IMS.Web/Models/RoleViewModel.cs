using Microsoft.AspNet.Identity.EntityFramework;
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
    [NotMapped] // prevent 'discriminator' error
    public class RoleViewModel: AspNetRole
    {
        public RoleViewModel()
        {
            this.Accesses = new List<AccessViewModel>();
        }

        public string Description { get; set; }

        [Display(Name = "Administrator")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? Created { get; set; }

        [Display(Name="Created By")]
        public string CreatedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? Modified { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        public List<AccessViewModel> Accesses { get; set; }

        public IEnumerable<IdentityUserRole> Users { get; set; }
    }
}