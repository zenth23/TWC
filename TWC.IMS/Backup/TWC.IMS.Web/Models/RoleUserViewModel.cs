using TWC.IMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    public class RoleUserViewModel : UserDetail
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string UserId { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}