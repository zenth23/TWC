using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(TWC.IMS.Portal.Startup))]

namespace TWC.IMS.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieName = "MyCustomAuthCookie",
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                ExpireTimeSpan = TimeSpan.FromDays(30),
                SlidingExpiration = true,
                CookieSecure = CookieSecureOption.Always,
                CookieHttpOnly = true,
                CookieManager = new ChunkingCookieManager()
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            ConfigureAuth(app);
        }
    }
}
