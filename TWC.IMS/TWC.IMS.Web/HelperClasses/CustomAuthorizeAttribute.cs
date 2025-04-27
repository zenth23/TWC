using Microsoft.AspNet.Identity.Owin;
using TWC.IMS.BL;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TWC.IMS.Web.HelperClasses
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private RolePermissions _rpBL = null;

        public string AccessName { get; set; }

        public string UserName { get; set; }

        private bool IsUserActive(HttpContextBase httpContext)
        {
            string username = httpContext.User.Identity.Name;
            username = string.IsNullOrEmpty(username) ? "Anonymous" : username;
            using (var udBL = new BL.UserDetails(username))
            {
                return TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => udBL.IsAccountActiveAsync(username));
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // check for permission
            if (this.AccessName == null) // allow class-level permission
                return true;

            bool isActive = this.IsUserActive(httpContext);
            if (!isActive)
                return false;

            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
                return false;

            var isAllowed = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => RolePermissionsHttpContextExtensions.HasPermissionAsync(httpContext, this.AccessName));
            return isAllowed;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonResult result = new JsonResult
                {
                    Data = new TWC.IMS.Models.HelperModels.ReturnMessageModel
                    {
                        HttpStatusCode = HttpStatusCode.Unauthorized,
                        Status = Common.HelperClasses.StatusType.ERROR,
                        Message = "Unauthorized request."
                    }
                };
                filterContext.Result = result;
            }
            else
            {
                string actionName = filterContext.RouteData.Values["action"].ToString();
                string controllerName = filterContext.RouteData.Values["controller"].ToString();
                string unauthorizedMessage = TWC.IMS.Common.Messages.NOT_AUTHORIZED;

                var exceptionContext = new ExceptionContext
                {
                    Exception = new HttpException(401, unauthorizedMessage),
                    RouteData = filterContext.RouteData,
                    HttpContext = filterContext.HttpContext
                };
                var result = ErrorHandleHelper.OopsMessage(exceptionContext);
                filterContext.Result = result.Item1;
            }
        }

        public async Task<KeyValuePair<string, IEnumerable<string>>> AuthorizedModulesAsync(HttpContextBase httpContext)
        {
            string username = httpContext.User.Identity.Name;
            using (_rpBL = new RolePermissions(username))
            {
                string rolesArray = "";
                var list = new List<string>();
                ApplicationUserManager userManager = httpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = await userManager.FindByNameAsync(username).ConfigureAwait(false);
                if (user != null)
                {
                    string userId = user.Id;
                    var roles = await userManager.GetRolesAsync(userId).ConfigureAwait(false);
                    if (roles.Any())
                    {
                        rolesArray = string.Join(",", roles);
                        var permissions = await _rpBL.GetListAsync().ConfigureAwait(false);
                        foreach (var role in roles)
                        {
                            permissions = permissions.Where(a => string.Compare(a.AspNetRole.Name, role, true) == 0);
                            foreach (var permission in permissions)
                            {
                                string accessName = $"{permission.ModuleAccess.Module.Name}.{permission.ModuleAccess.Access.Name}";
                                if (!list.Contains(accessName.ToLower()))
                                    list.Add(accessName.ToLower());
                            }
                        }
                    }
                }
                return new KeyValuePair<string, IEnumerable<string>>(rolesArray, list);
            }
        }
    }
}