using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    public class ModelStateSummary
    {
        public string PropertyName { get; set; }
        public string[] ErrorMessages { get; set; }
    }
}