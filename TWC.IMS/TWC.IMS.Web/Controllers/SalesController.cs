using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using TWC.IMS.BL;
using TWC.IMS.Models;
using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.Common.HelperClasses;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using System.IO;


namespace TWC.IMS.Web.Controllers
{
    public class SalesController : Controller
    {
        public ActionResult Index()
        {
          
            return View();
        }

    
    }
}