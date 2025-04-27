using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class SystemAdministrationController : BaseController
    {
        // GET: SystemAdministration
        [CustomAuthorize(AccessName = "SystemAdministration.CanView")]
        public ActionResult Index()
        {
            return View();
        }
    }
}