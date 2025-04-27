using TWC.IMS.Web.HelperClasses;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web
{
    public class FilterConfig
    {
        private FilterConfig() { }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters != null)
            {
                filters.Add(new HandleErrorAttribute());
                filters.Add(new CustomErrorHandler());
                filters.Add(new LogActionAttribute());
                filters.Add(new UserSessionActionFilter());
            }
        }
    }
}
