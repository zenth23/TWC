using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TWC.IMS.Web.HelperClasses
{
    public class UserSessionActionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Any())
                return;

            HttpContextBase httpContext = filterContext.HttpContext;
            if (httpContext.Session["USERNAME"] == null)
            {
                // FOR FORMS AUTH ONLY
                // use home/index for windows auth
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();

                if (string.Compare(controller, "account", true) == 0 &&
                    string.Compare(action, "login", true) == 0)
                {
                    return;
                }

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    JsonResult result = new JsonResult { Data = "Session timeout." };
                    filterContext.Result = result;
                }
                else
                {
                    FormsAuthentication.SignOut();
                    httpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    httpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                    httpContext.Session.Clear();
                    httpContext.Session.Abandon();

                    filterContext.Result = new ViewResult
                    {
                        ViewName = "SessionTimeout"
                    };
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}