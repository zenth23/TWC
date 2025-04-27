using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using TWC.IMS.Common;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.BL;
using TWC.IMS.Models;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace TWC.IMS.Web.Controllers
{
    public class BaseController : Controller
    {
        #region PRIVATE MEMBERS
        private ApplicationSignInManager _signInManager = null;
        private ApplicationUserManager _userManager = null;
        private ApplicationRoleManager _roleManager = null;


        #endregion

        public BL.Accesses _accessesBL = null;
        public BL.AccountManagers _accountManagersBL = null;
      
        public BL.AspNetRoles _aspNetRolesBL = null;
        public BL.AspNetUsers _aspNetUsersBL = null;

        public BL.AuditLogs _auditLogsBL = null;
        public BL.DatabaseArchivingLogs _databaseArchivingLogsBL = null;
        public BL.EmailLogs _emailLogsBL = null;
        public BL.ErrorLogs _errorLogsBL = null;
        public BL.ModuleAccesses _moduleAccessesBL = null;
        public BL.Modules _modulesBL = null;
        public BL.PasswordHistories _passwordHistoriesBL = null;
        public BL.Reports _reportsBL = null;
        public BL.Requests _requestsBL = null;
        public BL.RoleDetails _roleDetailsBL = null;
        public BL.RolePermissions _rolePermissionsBL = null;
        public BL.StatusSets _statusBL = null;
        public BL.SupportTool _supportToolBL = null;
        public BL.SystemConfigs _systemConfigsBL = null;
        //public BL.SystemNotifications _systemNotificationsBL = null;
        public BL.UserDetails _userDetailsBL = null;
        public BL.UserActivityLogs _userActivityLogsBL = null;

        // add more here
        public BL.Product_Inventory _inventoryBL = null;
        public BL.Products _productsBL = null;
        public BL.DemoSources _demoSourcesBL = null;
        public BL.ReportCaches _reportCachesBL = null;
        public BL.SalesTypes _SalesTypeBL = null;
        public BL.Locations _locationBL = null;
        public BL.producttypes _productTypeBL = null;
        public string UserRole
        {
            get
            {
                return Session["USERROLES"]?.ToString();
            }
        }

        public string SessionId
        {
            get
            {
                return this.HttpContext.Session.SessionID;
            }
        }


        public DateTime? SessionStart
        {
            get
            {
                return (DateTime?)Session["SESSION_START"];
            }
        }

        public int SessionTimeout
        {
            get
            {
                return Session.Timeout;
            }
        }

        public bool IsMobileDevice
        {
            get
            {
                return Request.Browser.IsMobileDevice;
            }
        }

        public string ClientIPAddress
        {
            get
            {
                return Request.UserHostAddress;
            }
        }

        public string UserAgent
        {
            get
            {
                return Request.UserAgent;
            }
        }

        protected BaseController() { }

        protected BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public Application AppInstance
        {
            get
            {
                return Application.Instance;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        protected async Task<bool> CanViewAsync(string module)
        {
            bool isAuthorized = await this.HttpContext.HasPermissionAsync($"{module.Pluralize()}.CanView");
            ViewData.Add($"CanView{module.Singularize()}", isAuthorized);
            return isAuthorized;
        }

        protected async Task<bool> CanAddAsync(string module)
        {
            bool isAuthorized = await this.HttpContext.HasPermissionAsync($"{module.Pluralize()}.CanAdd");
            ViewData.Add($"CanAdd{module.Singularize()}", isAuthorized);
            return isAuthorized;
        }

        protected async Task<bool> CanEditAsync(string module)
        {
            bool isAuthorized = await this.HttpContext.HasPermissionAsync($"{module.Pluralize()}.CanEdit");
            ViewData.Add($"CanEdit{module.Singularize()}", isAuthorized);
            return isAuthorized;
        }

        protected async Task<bool> CanDeleteAsync(string module)
        {
            bool isAuthorized = await this.HttpContext.HasPermissionAsync($"{module.Pluralize()}.CanDelete");
            ViewData.Add($"CanDelete{module.Singularize()}", isAuthorized);
            return isAuthorized;
        }

        public async Task<bool> CanUploadAsync(string module)
        {
            bool isAuthorized = await this.HttpContext.HasPermissionAsync($"{module}.CanUpload").ConfigureAwait(false);
            ViewData.Add($"CanUpload{module.Singularize()}", isAuthorized);
            return isAuthorized;
        }

        public async Task<bool> CanDownloadAsync(string module)
        {
            bool isAuthorized = await this.HttpContext.HasPermissionAsync($"{module}.CanDownload").ConfigureAwait(false);
            ViewData.Add($"CanDownload{module.Singularize()}", isAuthorized);
            return isAuthorized;
        }

        public async Task<bool> IsCurrentUserAdminAsync()
        {
            var userRoles = await UserManager.GetRolesAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            using (_roleDetailsBL = new RoleDetails(User.Identity.Name))
            {
                var roles = await _roleDetailsBL.GetListAsync().ConfigureAwait(false);
                roles = roles.Where(a => userRoles.Contains(a.AspNetRole.Name, StringComparer.CurrentCultureIgnoreCase));
                bool isAdmin = roles.Any(a => a.IsAdmin);
                ViewBag.IsCurrentUserAdmin = isAdmin;
                return isAdmin;
            }
        }

        public void LogOffSession()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
        }

        public void LogOffSession(string userId)
        {
            UserManager.UpdateSecurityStamp(userId);
            LogOffSession();
        }

        public async Task LogPasswordAsync(string plainTextPassword, string userId, string currentLoggedInUsername, bool? isTemporaryPassword = false)
        {
            try
            {
                int retryCounter = 0;
                // hash new password
                string passwordHash = UserManager.PasswordHasher.HashPassword(plainTextPassword);
                PasswordVerificationResult r = PasswordVerificationResult.Failed;
                while (r == PasswordVerificationResult.SuccessRehashNeeded || r == PasswordVerificationResult.Failed)
                {
                    if (retryCounter < 6)
                    {
                        r = UserManager.PasswordHasher.VerifyHashedPassword(passwordHash, plainTextPassword);
                        if (r == PasswordVerificationResult.Success)
                        {
                            var obj = new PasswordHistory();
                            obj.PasswordHistory_AspNetUser = userId;
                            obj.PasswordHash = passwordHash;
                            obj.IsTemporaryPassword = isTemporaryPassword.Value;

                            using (_passwordHistoriesBL = new PasswordHistories(currentLoggedInUsername))
                            {
                                await _passwordHistoriesBL.InsertAsync(obj).ConfigureAwait(false);
                            }
                        }
                    }
                    else
                    {
                        break;
                        throw new Exception("There was a problem encrypting your new password.");
                    }
                    retryCounter++;
                }
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, currentLoggedInUsername);
            }
        }

        public async Task LogErrorAsync(MessageType messageType, Exception ex, string currentLoggedInUsername, string paramData = null)
        {
            switch (messageType)
            {
                case MessageType.ERROR:
                    await Logger.ErrorAsync(ex.Message, currentLoggedInUsername, AppInstance.ApplicationVersion, UserRole,
                                            AppInstance.Environment, this.ClientIPAddress, this.UserAgent, ex,
                                            isMobileDevice: IsMobileDevice, paramData: paramData).ConfigureAwait(false);
                    break;
                case MessageType.INFORMATION:
                    await Logger.InformationAsync(ex.Message, currentLoggedInUsername, AppInstance.ApplicationVersion, UserRole,
                                                  AppInstance.Environment, this.ClientIPAddress, this.UserAgent,
                                                  isMobileDevice: IsMobileDevice, paramData: paramData).ConfigureAwait(false);
                    break;
                case MessageType.WARNING:
                    await Logger.WarningAsync(ex.Message, currentLoggedInUsername, AppInstance.ApplicationVersion, UserRole,
                                              AppInstance.Environment, this.ClientIPAddress, this.UserAgent,
                                              isMobileDevice: IsMobileDevice, paramData: paramData).ConfigureAwait(false);
                    break;
            }
        }

        public void LogUserAttempt(string activity)
        {
            try
            {
                string methodType = this.HttpContext.Request.HttpMethod;
                string absoluteUrl = this.HttpContext.Request.Url.AbsoluteUri;
                string userAgent = this.UserAgent;
                string ipAddress = this.ClientIPAddress;
                string formData = JsonConvert.SerializeObject(this.HttpContext.Request.Form);

                if (_userActivityLogsBL == null)
                    _userActivityLogsBL = new UserActivityLogs(User.Identity.Name);

                var _ = _userActivityLogsBL.InsertAsync(methodType, absoluteUrl, activity, ipAddress, userAgent, AppInstance.ApplicationVersion,
                                                        IsMobileDevice, SessionId, SessionStart, SessionTimeout, UserRole, formData).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                var _ = LogErrorAsync(MessageType.ERROR, ex, User.Identity.Name);
            }
        }

        /// <summary>
        /// Being called by RedirectToAction methods. Redirects the request to the supplied url params
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="c">controller</param>
        /// <param name="a">action</param>
        /// <param name="anchor">ie: #account</param>
        /// <returns></returns>
        public async Task<ActionResult> AutoSignInUser(string userId, string c, string a, string anchor = "")
        {
            ApplicationUser user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, false, false).ConfigureAwait(false);
            }
            return Redirect(Url.RouteUrl(new { controller = c, action = a }) + anchor);
        }
    }
}