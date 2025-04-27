using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TWC.IMS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var instance = TWC.IMS.Web.HelperClasses.Application.Instance;

            // Fix for Qualys finding: "150081 - X-Frame-Options header is not set"
            // This will suppress the extra x-frame-options response header generated in every request
            // When set to true, X-FRAME-OPTIONS must be set on IIS/web.config
            System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            // Disable sending the X-AspNetMvc-Version header
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // A6 Security Misconfiguration
        protected void Application_BeginRequest()
        {
            //#if !DEBUG
            //            // SECURE: Ensure any request is returned over SSL/TLS in production
            //            if (!Request.IsLocal && !Context.Request.IsSecureConnection)
            //            {
            //                var redirect = Context.Request.Url.ToString()
            //                                                  .ToLower(CultureInfo.CurrentCulture)
            //                                                  .Replace("http:", "https:");
            //                Response.Redirect(redirect);
            //            }
            //#endif
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // initialize session objects here
            // this will make the session values unique per user per session

            // applicable to WinAuth only. For FOrmsAuth, session is started after successful login
            //Context.Session["SESSION_START"] = DateTime.Now;
            Debug.WriteLine($"SESSION_START: {DateTime.Now}");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // initialize session objects here
            // this will make the session values unique per user per session            
            Debug.WriteLine($"SESSION_END: {DateTime.Now}");
        }

        // will only be triggered when the OutputCache params are: 
        // [OutputCache(Duration = X, VaryByCustom = "username", Location = System.Web.UI.OutputCacheLocation.Server)]
        // [OutputCache(CacheProfile = "StandardCache_0060_Server", VaryByCustom = "username")]
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom.Equals("username", StringComparison.OrdinalIgnoreCase))
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    string username = context.User.Identity.Name;
                    return username;
                }
                return null;
            }
            return base.GetVaryByCustomString(context, custom);
        }
    }
}
