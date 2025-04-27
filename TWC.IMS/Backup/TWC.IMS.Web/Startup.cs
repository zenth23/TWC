using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TWC.IMS.Web.Startup))]
namespace TWC.IMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //// SIGNALR
            app.MapSignalR();
        }
    }
}
