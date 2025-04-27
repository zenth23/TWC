using Newtonsoft.Json;
using TWC.IMS.BL;
using TWC.IMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.HelperClasses
{
    /// <summary>
    /// For authorized ActionResults calls only (not anonymous)
    /// </summary>
    public class LogActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipLogActionFilterAttribute), false).Any())
                return;

            var requestContext = filterContext.RequestContext;
            string username = filterContext.HttpContext.User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {
                var controller = requestContext.RouteData.Values["Controller"].ToString();
                var action = requestContext.RouteData.Values["Action"].ToString();
                var method = requestContext.HttpContext.Request.HttpMethod;
                var url = requestContext.HttpContext.Request.Url.AbsoluteUri;

                var model = new UserActivityLog();
                model.Activity = $"{controller} > {action}";
                model.AbsoluteUrl = url;
                model.MethodType = method;
                model.ClientIPAddress = requestContext.HttpContext.Request.UserHostAddress;
                model.UserAgent = requestContext.HttpContext.Request.UserAgent;
                
                var appInstance = Application.Instance;
                model.AppVersion = appInstance.ApplicationVersion;
                model.SessionStart = (DateTime?)filterContext.HttpContext.Session["SESSION_START"];
                model.IsMobileDevice = filterContext.HttpContext.Request.Browser.IsMobileDevice;
                model.SessionId = filterContext.HttpContext.Session.SessionID;
                model.SessionTimeout = filterContext.HttpContext.Session.Timeout;
                model.UserRole = filterContext.HttpContext.Session["USERROLES"]?.ToString();
                model.FormData = JsonConvert.SerializeObject(filterContext.HttpContext.Request.Form);

                // insert log
                using (var ualBL = new UserActivityLogs(username))
                {
                    TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => ualBL.InsertAsync(model));
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}