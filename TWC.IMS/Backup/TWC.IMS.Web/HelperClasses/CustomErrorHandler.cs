using TWC.IMS.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.HelperClasses
{
    public class CustomErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var ctx = filterContext.HttpContext;
            string username = filterContext.HttpContext.User?.Identity.Name ?? "Anonymous";

            try
            {
                filterContext.ExceptionHandled = true;

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    JsonResult result = new JsonResult
                    {
                        Data = new TWC.IMS.Models.HelperModels.ReturnMessageModel
                        {
                            HttpStatusCode = HttpStatusCode.InternalServerError,
                            Status = Common.HelperClasses.StatusType.ERROR,
                            Message = TWC.IMS.Common.Messages.SOMETHING_WENT_WRONG
                        }
                    };
                    filterContext.Result = result;
                }
                else
                {
                    if (filterContext.Exception is HttpAntiForgeryException)
                    {
                        filterContext.Exception = new Exception(TWC.IMS.Common.Messages.SOMETHING_WENT_WRONG);
                    }

                    var oops = ErrorHandleHelper.OopsMessage(filterContext);
                    filterContext.Result = oops.Item1;
                    //string controllerName = oops.Item2;
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                var _ = Logger.LogToEventViewer(message);
            }
            finally
            {
                // log error here
                // will trigger fire and forget
                Exception ex = filterContext.Exception;
                var appInstance = Application.Instance;
                string clientIPAddress = filterContext.HttpContext.Request.UserHostAddress;
                string userAgent = filterContext.HttpContext.Request.UserAgent;
                var _ = Logger.ErrorAsync(ex.Message, username, appInstance.ApplicationVersion, ctx.Session["USERROLES"]?.ToString(), appInstance.Environment, clientIPAddress, userAgent, ex, isMobileDevice: ctx.Request.Browser.IsMobileDevice);
            }
        }
    }
}