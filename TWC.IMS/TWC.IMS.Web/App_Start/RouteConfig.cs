using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TWC.IMS.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Maintenance",
                url: "maintenance/{action}",
                defaults: new { controller = "usermaintenance", action = "index" }
            );

            routes.MapRoute(
                name: "UserMaintenanceDetails",
                url: "maintenance/{action}/{type}/{key}",
                defaults: new { controller = "usermaintenance", action = "details", type = "u", key = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UserMaintenanceCreateUserRole",
                url: "maintenance/{action}/{type}",
                defaults: new { controller = "usermaintenance", action = "details", type = "u" }
            );

            routes.MapRoute(
                name: "Default2",
                url: "{controller}/{action}/{key}",
                defaults: new { controller = "home", action = "index", key = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "account", action = "login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
    name: "SalesOrderDetails",
    url: "SalesOrders/Details/{id}",
    defaults: new { controller = "SalesOrders", action = "Details", id = UrlParameter.Optional }
);

        }
    }
}
