using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.HelperModels
{
    public class RoleMasterdataReportModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Administrator { get; set; }
        public bool Active { get; set; }
    }
}
