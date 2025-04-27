using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class TotpBasedAuthenticatorViewModel
    {
        public string AppName { get; set; }
        public string Code { get; set; }
        public string SecretKey { get; set; }
        public string BarcodeUrl { get; set; }
        public string Provider { get; set; }
    }
}