using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.HelperClasses
{
    public static class ErrorHandleHelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns>Item1 - View, Item2 - Controller Name</returns>
        public static Tuple<ViewResult, string> OopsMessage(ExceptionContext filterContext)
        {
            string oopsImagesPath = ConfigurationManager.AppSettings["OOPS_IMAGES_PATH"];
            string exceptionHeader = "OOPS";
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string controllerName = filterContext.RouteData.Values["controller"].ToString();

            oopsImagesPath = filterContext.HttpContext.Server.MapPath(oopsImagesPath);
            var model = new HandleErrorInfoViewModel(filterContext.Exception, controllerName, actionName, exceptionHeader, oopsImagesPath);
            return Tuple.Create(new ViewResult()
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(model)
            }, controllerName);
        }
    }
}