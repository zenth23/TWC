using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    public class PermissionViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public DateTimeOffset RowVersion { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}