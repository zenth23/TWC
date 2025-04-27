using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Web.HelperClasses;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class HomeController : BaseController
    {
        [Authorize]
        public async Task<ActionResult> Index()
        {
            ViewBag.FirstName = await User.Identity.GetFirstNameAsync().ConfigureAwait(false);
            await ReportHelpers.GetReportExpirationAsync(this.HttpContext).ConfigureAwait(false);
            
            return View();
        }

        [AllowAnonymous]
        public ActionResult PrivacyStatement()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CreatePSCookie()
        {
            HttpCookie psCookie = HttpContext.Response.Cookies["privacyCookie"] ?? new HttpCookie("privacyCookie");
            psCookie.Value = "AGREED";
            psCookie.Expires = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddDays(1);
            //Response.SetCookie(psCookie);
            this.ControllerContext.HttpContext.Response.Cookies.Add(psCookie);

            return Json(new { Agreed = true }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetPSCookie()
        {
            bool agreed = false;
            HttpCookie psCookie = Request.Cookies["privacyCookie"];
            if (psCookie != null && psCookie.Value == "AGREED")
                agreed = true;

            return Json(new { Agreed = agreed }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult CookiePolicy()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CreateCNCookie()
        {
            HttpCookie cnCookie = HttpContext.Response.Cookies["cookieNoticeCookie"] ?? new HttpCookie("cookieNoticeCookie");
            cnCookie.Value = "AGREED";
            cnCookie.Expires = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddDays(1);
            //Response.SetCookie(psCookie);
            this.ControllerContext.HttpContext.Response.Cookies.Add(cnCookie);

            return Json(new { Agreed = true }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetCNCookie()
        {
            bool agreed = false;
            HttpCookie cnCookie = Request.Cookies["cookieNoticeCookie"];
            if (cnCookie != null && cnCookie.Value == "AGREED")
                agreed = true;

            return Json(new { Agreed = agreed }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        [SkipLogActionFilter]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult
            {
                Data = "Beat generated"
            };
        }

        public ActionResult VersionHistory()
        {
            return View();
        }
    }
}