using TWC.IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    public class AccessModuleViewModel : Module
    {
        public AccessModuleViewModel()
        {
            this.Accesses = new List<AccessViewModel>();
        }

        public byte[] AccessModuleRowVersion { get; set; }

        public List<AccessViewModel> Accesses { get; set; }
    }
}