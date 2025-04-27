using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TWC.IMS.Portal.Models
{
    // Models returned by MeController actions.
    public class GetViewModel
    {
        public string Hometown { get; set; }
    }
}