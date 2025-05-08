using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class CatalogUploadViewModel
    {
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
    }
}