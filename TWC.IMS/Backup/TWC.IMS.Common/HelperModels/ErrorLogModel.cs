using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperModels
{
    public class ErrorLogModel
    {
        public string ApplicationVersion { get; set; }
        public string UserRole { get; set; }
        public string Environment { get; set; }
        public bool IsMobileDevice { get; set; }
    }
}
