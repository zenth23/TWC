
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using TWC.IMS.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.IO;

namespace TWC.IMS.Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public BL.Products _productsBL = null;
        public ActionResult Index()
        {
            return View();
        }
      
  

    }
}
