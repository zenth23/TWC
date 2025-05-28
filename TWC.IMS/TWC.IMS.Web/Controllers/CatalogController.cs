using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TWC.IMS.Web.Models;
using System.Web.Security;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.BL;
using TWC.IMS.Models;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Security.Principal;
using TWC.IMS.Common.HelperClasses;
using System.Diagnostics;

namespace TWC.IMS.Web.Controllers
{
    public class CatalogController : Controller
    {
        // GET: Catalog
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}