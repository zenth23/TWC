using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using TWC.IMS.Web.Models;

[assembly: OwinStartupAttribute(typeof(TWC.IMS.Web.Startup))]
namespace TWC.IMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                ExpireTimeSpan = TimeSpan.FromDays(14),  // Keep user logged in for 14 days if "Remember Me" is checked
                SlidingExpiration = true,  // Reset expiration on activity
                CookieSecure = CookieSecureOption.Always, // Ensure secure cookies (HTTPS only)
                CookieHttpOnly = true,  // Prevent JavaScript access to cookies
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                            validateInterval: TimeSpan.FromMinutes(30),  // Revalidate security stamp every 30 mins
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            ConfigureAuth(app);

            //// SIGNALR
            app.MapSignalR();
        }
    }
}
