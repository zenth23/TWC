// REQUEST FOR REMOVAL

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TWC.IMS.Common;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DbModels = TWC.IMS.Models;
using System.Diagnostics;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class RequestsController : BaseController
    {
        // GET: Requests
        public async Task<ActionResult> Index()
        {
            return await Task.FromResult(View());
        }
        
    }
}