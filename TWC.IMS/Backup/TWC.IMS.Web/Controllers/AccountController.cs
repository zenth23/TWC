using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TWC.IMS.Web.Models;
using System.Web.Security;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.BL;
using TWC.IMS.Models;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Security.Principal;
using TWC.IMS.Common.HelperClasses;
using System.Diagnostics;

namespace TWC.IMS.Web.Controllers
{
    public class AccountController : BaseController
    {
        #region PRIVATE MEMBERS

        private Task AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return Task.FromResult(0);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task LogOffSessionAsync(string userId)
        {
            try
            {
                Session["USERNAME"] = null;

                try
                {
                    FormsAuthentication.SignOut();

                    using (var signalRBL = new BL.SignalRConnection(userId))
                    {
                        await signalRBL.DeleteAllAsync(userId).ConfigureAwait(false);
                        //var connectionId = Hubs.SystemNotificationHub.GetConnectionId();
                        //await signalRBL.DeleteAsync(connectionId).ConfigureAwait(false);
                    }
                }
                catch { }

                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                if (!string.IsNullOrEmpty(userId))
                    await UserManager.UpdateSecurityStampAsync(userId).ConfigureAwait(false);

                Session.Clear();
                Session.Abandon();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private Task TriggerFailCount(string username, int failedAccessLimit, bool isRegUser, ref int accessFailedCount)
        {
            if (!isRegUser)
            {
                if (TempData["ACCESS_FAILED_COUNT"] != null)
                {
                    string[] values = TempData["ACCESS_FAILED_COUNT"].ToString().Split(':');
                    int count = Convert.ToInt32(values[1]);
                    var unregUser = values[0];
                    if (string.Compare(username, unregUser, true) == 0)
                        accessFailedCount = count + 1;
                    else
                        accessFailedCount = 1; // initial
                }
                else
                    accessFailedCount = 1; // initial

                TempData["ACCESS_FAILED_COUNT"] = $"{username}:{accessFailedCount}";
            }

            if (accessFailedCount >= failedAccessLimit)
                ModelState.AddModelError("", "Your account has been locked out. Please contact your system administrator.");
            else
                ModelState.AddModelError("", $"Invalid login attempt {accessFailedCount}/{failedAccessLimit}.");

            return Task.FromResult(0);
        }

        private async Task<string> GenerateRandomPasswordAsync()
        {
            bool isValid = false;
            string tempPassword = "";
            while (!isValid)
            {
                tempPassword = Membership.GeneratePassword(10, 1);
                // check if valid                        
                var r = await UserManager.PasswordValidator.ValidateAsync(tempPassword).ConfigureAwait(false);
                isValid = r.Succeeded;
            }

            return tempPassword;
        }

        private async Task<bool> CreateSigninNotificationAsync(UserDetail udObj, string username)
        {
            try
            {
                using (_userActivityLogsBL = new BL.UserActivityLogs(username))
                using (_systemConfigsBL = new BL.SystemConfigs(username))
                {
                    var wfEngine = new WorkflowEngine(username, UserRole, IsMobileDevice, ClientIPAddress, UserAgent);
                    var notifHub = new Hubs.SystemNotificationHub();

                    // SUCCESS LOGIN
                    var notifObj = new TWC.IMS.Models.SystemNotification();
                    notifObj.Title = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.LAST_SUCCESS_LOGIN_TITLE).ConfigureAwait(false);
                    notifObj.Caption = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.LAST_SUCCESS_LOGIN_CAPTION).ConfigureAwait(false);
                    notifObj.Description = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.LAST_SUCCESS_LOGIN_DESC).ConfigureAwait(false);
                    notifObj.Description = notifObj.Description.Replace("{LastLoginDatetime}", udObj.LastLoginDatetime.HasValue ? udObj.LastLoginDatetime.Value.ToString(Common.StringFormats.DATETIME_FORMAT_SHORT_1) : "");
                    notifObj.IsViewed = false;
                    notifObj.SystemNotification_UserDetail = udObj.Id;
                    notifObj.UserDetail = udObj;
                    notifObj.Url = "#";

                    await wfEngine.SendSystemNotificationAsync(notifHub, notifObj).ConfigureAwait(false);

                    // FAILED ATTEMTPS
                    var failedStr = "Failed attempt";
                    var failedAttempts = await _userActivityLogsBL.GetListAsync(failedStr, username, udObj.LastLoginDatetime, DateTime.Now).ConfigureAwait(false);

                    var title = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.LAST_FAILED_LOGIN_TITLE).ConfigureAwait(false);
                    var caption = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.LAST_FAILED_LOGIN_CAPTION).ConfigureAwait(false);
                    var desc = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.LAST_FAILED_LOGIN_DESC).ConfigureAwait(false);

                    foreach (var attempt in failedAttempts)
                    {
                        notifObj = new TWC.IMS.Models.SystemNotification();
                        notifObj.Title = title;
                        notifObj.Caption = caption.Replace("{AttemptDate}", attempt.Created.Value.ToString(Common.StringFormats.DATETIME_FORMAT_LONG_1));
                        notifObj.Description = desc.Replace("{IPAddress}", attempt.ClientIPAddress)
                                                   .Replace("{UserAgent}", attempt.UserAgent);
                        notifObj.IsViewed = false;
                        notifObj.SystemNotification_UserDetail = udObj.Id;
                        notifObj.UserDetail = udObj;
                        notifObj.Url = "#";

                        await wfEngine.SendSystemNotificationAsync(notifHub, notifObj);
                    }

                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);
                return await Task.FromResult(false);
            }
        }

        private TWC.IMS.Common.Mailer MailerInstance()
        {
            var mailerInstance = TWC.IMS.Common.Mailer.Instance;
            mailerInstance.ApplicationVersion = this.AppInstance.ApplicationVersion;
            mailerInstance.Environment = this.AppInstance.Environment;
            mailerInstance.ClientIPAddress = this.ClientIPAddress;
            mailerInstance.IsMobileDevice = this.IsMobileDevice;
            mailerInstance.UserAgent = this.UserAgent;
            mailerInstance.UserRole = this.UserRole;
            return mailerInstance;
        }
        #endregion

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("index", "home");
                else
                    return RedirectToLocal(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AllowXRequestsEveryXSeconds(Name = "Login", Requests = 3, Seconds = 60)]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
          
            if (!ModelState.IsValid)
                return View(model);

            bool agreed = false;
            HttpCookie psCookie = Request.Cookies["privacyCookie"];
            if (psCookie != null)
                agreed = true;

            if (!agreed)
            {
                HttpCookie cookie = new HttpCookie("privacyCookie")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                this.ControllerContext.HttpContext.Response.Cookies.Set(cookie);
                ModelState.AddModelError("", "You must agree first on our privacy statement.");
                return View(model);
            }

            agreed = false;
            HttpCookie cnCookie = Request.Cookies["cookieNoticeCookie"];
            if (cnCookie != null)
                agreed = true;

            if (!agreed)
            {
                HttpCookie cookie = new HttpCookie("cookieNoticeCookie")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                this.ControllerContext.HttpContext.Response.Cookies.Set(cookie);

                ModelState.AddModelError("", "You must accept the cookie notice which appears below in order to login.");
                return View(model);
            }

            string modelUsername = model.Username.Trim();

            int failedAccessLimit = UserManager.MaxFailedAccessAttemptsBeforeLockout;
            int accessFailedCount = 0;
            var user = await UserManager.FindByNameAsync(modelUsername).ConfigureAwait(false);
            if (user == null)
            {
                await TriggerFailCount(modelUsername, failedAccessLimit, false, ref accessFailedCount);
                return View(model);
            }

            string userId = user.Id;

            using (_systemConfigsBL = new BL.SystemConfigs(modelUsername))
            using (_roleDetailsBL = new BL.RoleDetails(modelUsername))
            using (_userDetailsBL = new BL.UserDetails(modelUsername))
            using (_userActivityLogsBL = new UserActivityLogs(modelUsername))
            using (_accountManagersBL = new AccountManagers(modelUsername))
            {
                LogUserAttempt("Login attempt");

                // check if active
                bool isActive = await _userDetailsBL.IsAccountActiveAsync(modelUsername).ConfigureAwait(false);
                if (isActive)
                {
                    bool hasActiveRole = await _roleDetailsBL.HasActiveRoleAsync(user.Roles.Select(a => a.RoleId)).ConfigureAwait(false);
                    if (!hasActiveRole)
                    {
                        ModelState.AddModelError("", "You have no active role assigned. Please contact your system administrator.");
                        return View(model);
                    }

                    // check if not expired
                    var udObj = await _userDetailsBL.GetByUserIdAsync(userId).ConfigureAwait(false);
                    if (udObj != null)
                    {
                        var result = await SignInManager.PasswordSignInAsync(modelUsername, model.Password, model.RememberMe, shouldLockout: true).ConfigureAwait(false);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                // check for password if temporary
                                using (var phBL = new PasswordHistories(user.UserName))
                                {
                                    var ph = await phBL.GetCurrentPasswordAsync(userId).ConfigureAwait(false);
                                    if (ph != null)
                                    {
                                        bool isExpired = await _accountManagersBL.IsPasswordExpiredAsync(userId).ConfigureAwait(false);
                                        if (ph.IsTemporaryPassword || isExpired)
                                        {
                                            LogUserAttempt("Redirected to password reset page");

                                            await LogOffSessionAsync(userId).ConfigureAwait(false);
                                            // redirect to reset password page
                                            string code = await UserManager.GeneratePasswordResetTokenAsync(userId).ConfigureAwait(false);
                                            return RedirectToActionPermanent("changepassword", "manage", new { userId = userId, code = code });
                                        }
                                        else
                                        {
                                            // check for roles
                                            var roles = await UserManager.GetRolesAsync(userId).ConfigureAwait(false);
                                            if (roles.Count <= 0)
                                            {
                                                // remove session immediately
                                                await LogOffSessionAsync(userId).ConfigureAwait(false);

                                                throw new Exception("No role assigned to you. Please contact your system administrator.");
                                            }
                                        }
                                    }

                                    Session["USERNAME"] = User.Identity.Name;
                                    Session["SESSION_START"] = DateTime.Now;
                                    LogUserAttempt("Login successful");

                                    if (udObj != null)
                                    {
                                        var __ = CreateSigninNotificationAsync(udObj, modelUsername);

                                        int udId = udObj.Id;
                                        await _userDetailsBL.SetUserLastLoginDatetimeAsync(udId).ConfigureAwait(false);
                                    }

                                    // reset AccessFailedCount
                                    await UserManager.ResetAccessFailedCountAsync(userId).ConfigureAwait(false);
                                    return RedirectToLocal(returnUrl);
                                }

                            case SignInStatus.LockedOut:
                                ModelState.AddModelError("", TWC.IMS.Common.Messages.ACCOUNT_LOCKED_OUT);
                                return View(model);

                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                            case SignInStatus.Failure:

                            default:
                                LogUserAttempt("Failed attempt");

                                accessFailedCount = user.AccessFailedCount;
                                var _ = TriggerFailCount(modelUsername, failedAccessLimit, true, ref accessFailedCount);
                                return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", TWC.IMS.Common.Messages.ACCOUNT_NO_DETAILS);
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", TWC.IMS.Common.Messages.ACCOUNT_NOT_ACTIVE);
                    return View(model);
                }
            }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync().ConfigureAwait(false))
            {
                throw new Exception("User not verified.");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //if (string.Compare(model.Provider, "Phone Code", true) == 0)
            //{
            //    string username = Session["TEMPORARY_USERNAME"]?.ToString();
            //    string phoneNumber = ;
            //    string sid = ;
            //    string code = ;
            //    bool isValid = await TWC.IMS.Common.SmsService.ValidateSmsOtpCodeAsync(username, phoneNumber, sid, code).ConfigureAwait(false);
            //    if (isValid)
            //    {
            //        Session["USERNAME"] = User.Identity.Name;
            //        return RedirectToLocal(model.ReturnUrl);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "Invalid code.");
            //        return View(model);
            //    }
            //}
            //else
            //{
            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser).ConfigureAwait(false);
            switch (result)
            {
                case SignInStatus.Success:
                    Session["USERNAME"] = User.Identity.Name;
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
            //}
        }

        ////
        //// GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password).ConfigureAwait(false);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false).ConfigureAwait(false);

        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id).ConfigureAwait(false);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // var _ = UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>").ConfigureAwait(false);

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                throw new NullReferenceException("Unknown user.");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code).ConfigureAwait(false);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id).ConfigureAwait(false);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                using (_systemConfigsBL = new SystemConfigs(user.UserName))
                {
                    string subject = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_FORGOTPASSWORD).ConfigureAwait(false);
                    string body = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_FORGOTPASSWORD).ConfigureAwait(false);
                    body = body.Replace("{username}", user.UserName)
                               .Replace("{callbackUrl}", callbackUrl);

                    // let this method return a task and do its job but don't wait for a result
                    // so any code below will execute in parallel #FireAndForget
                    var mailerInstance = MailerInstance();
                    var _ = mailerInstance.SendMailAsync("FORGOT PASSWORD", user.UserName, subject, body, new[] { model.Email });
                }
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            var model = new ResetPasswordViewModel();
            model.Code = code;
            model.UserId = Request.QueryString["userId"];

            if (code == null)
                throw new NullReferenceException("Unknown code.");
            else
                return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AllowXRequestsEveryXSeconds(Name = "Login", Requests = 3, Seconds = 60)]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(model.UserId).ConfigureAwait(false);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            using (_accountManagersBL = new AccountManagers(user.UserName))
            {
                bool isInUse = await _accountManagersBL.IsPasswordInUseAsync(model.UserId, model.Password).ConfigureAwait(false);
                if (!isInUse)
                {
                    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password.Trim()).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        // set timestamp right after the reset password method
                        DateTime timestamp = DateTime.Now;

                        await LogPasswordAsync(model.Password.Trim(), user.Id, user.UserName).ConfigureAwait(false);

                        // tell identity to force logout this user
                        await UserManager.UpdateSecurityStampAsync(model.UserId).ConfigureAwait(false);

                        // send text msg about the change
                        var smsService = ((SmsService)UserManager.SmsService);
                        smsService.Username = User.Identity.Name;
                        smsService.IsMobileDevice = Request.Browser.IsMobileDevice;
                        smsService.UserRole = this.UserRole;
                        smsService.UserAgent = this.UserAgent;
                        smsService.ClientIPAddress =this.ClientIPAddress ;

                        var _ = smsService.SendPasswordChangeNotifAsync(user, timestamp);

                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }
                    await AddErrors(result);
                }
                else
                    ModelState.AddModelError("", "Password already in use. Please try another one.");

                return View(model);
            }
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [CustomAuthorize(AccessName = "Users.CanReset")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForcePasswordReset(string userId)
        {
            try
            {
                // generate new temp password
                string tempPassword = await GenerateRandomPasswordAsync();

                // update user password
                string token = await UserManager.GeneratePasswordResetTokenAsync(userId).ConfigureAwait(false);
                var result = await UserManager.ResetPasswordAsync(userId, token, tempPassword).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    // log password to password histories
                    await LogPasswordAsync(tempPassword, userId, User.Identity.Name, true).ConfigureAwait(false);

                    var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                    if (user != null)
                    {
                        // tell identity to force logout this user
                        await UserManager.UpdateSecurityStampAsync(userId).ConfigureAwait(false);

                        // send email to user
                        var urlBuilder = new UriBuilder(Request.Url.AbsoluteUri) { Path = Url.Action("login", "account") };
                        string loginUrl = urlBuilder.ToString();

                        using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
                        {
                            string subject = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_PASSWORDRESET).ConfigureAwait(false);
                            string body = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_PASSWORDRESET).ConfigureAwait(false);
                            body = body.Replace("{username}", user.UserName)
                                       .Replace("{password}", tempPassword)
                                       .Replace("{loginurl}", loginUrl);

                            // send email notification to user for the temporary password
                            // let this method return a task and do its job but don't wait for a result
                            // so any code below will execute in parallel #FireAndForget
                            var mailerInstance = MailerInstance();
                            var _ = mailerInstance.SendMailAsync("PASSWORD RESET", User.Identity.Name, subject, body, new[] { user.Email });
                        }
                        return Json(new { Status = "SUCCESS", Message = $"Password successfully reset. New password has been sent via email." });
                    }
                    else
                        return Json(new { Status = "ERROR", Message = "Unknown user" });
                }
                else
                {
                    string msg = "";
                    foreach (var err in result.Errors)
                    {
                        msg += err + "\n";
                    }
                    return Json(new { Status = "ERROR", Message = msg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERROR", Message = ex.Message });
            }
        }

        [CustomAuthorize(AccessName = "Users.CanLock")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LockUser(string userId)
        {
            try
            {
                // check if the account is already locked
                var isLocked = await UserManager.IsLockedOutAsync(userId).ConfigureAwait(false);
                if (isLocked)
                    return Json(new { Status = "ERROR", Message = "Account is already locked out." });

                string oldValue = null;
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                if (user != null)
                    oldValue = user.LockoutEndDateUtc == null ? null : user.LockoutEndDateUtc.Value.ToString();

                var result = await UserManager.SetLockoutEnabledAsync(userId, true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    result = await UserManager.SetLockoutEndDateAsync(userId, DateTime.Now.AddDays(365)).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        // update status
                        using (_userDetailsBL = new UserDetails(User.Identity.Name))
                        {
                            await _userDetailsBL.UpdateStatusLockUserAsync(userId).ConfigureAwait(false);
                        }

                        // tell identity to force logout this user
                        await UserManager.UpdateSecurityStampAsync(userId).ConfigureAwait(false);

                        // record change activity
                        user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                        if (user != null)
                        {
                            using (_auditLogsBL = new AuditLogs(User.Identity.Name))
                            {
                                string newValue = user.LockoutEndDateUtc == null ? null : user.LockoutEndDateUtc.Value.ToString();
                                var _ = _auditLogsBL.CreateAspNetUsersLockoutEndDateUtcModifiedEventAsync(oldValue, newValue, userId);
                            }
                        }

                        return Json(new { Status = "SUCCESS", Message = $"Account has been locked out successfully." });
                    }
                }

                string msg = "";
                foreach (var err in result.Errors)
                {
                    msg += err + "\n";
                }
                return Json(new { Status = "ERROR", Message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERROR", Message = ex.Message });
            }
        }

        [CustomAuthorize(AccessName = "Users.CanUnlock")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnlockUser(string userId)
        {
            try
            {
                // check if the account is already locked
                var isLocked = await UserManager.IsLockedOutAsync(userId).ConfigureAwait(false);
                if (!isLocked)
                    return Json(new { Status = "ERROR", Message = "Account is already unlocked." });

                string oldValue = null;
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                if (user != null)
                    oldValue = user.LockoutEndDateUtc == null ? null : user.LockoutEndDateUtc.Value.ToString();

                // make sure LockoutEnabled is set to true
                await UserManager.SetLockoutEnabledAsync(userId, true).ConfigureAwait(false);
                // then change LockoutEndDate to an earlier date than today
                var result = await UserManager.SetLockoutEndDateAsync(userId, DateTime.Now.AddDays(-365)).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    var r = await UserManager.ResetAccessFailedCountAsync(userId).ConfigureAwait(false);
                    user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                    if (user != null)
                    {
                        // record change activity
                        using (_auditLogsBL = new AuditLogs(User.Identity.Name))
                        {
                            string newValue = user.LockoutEndDateUtc == null ? null : user.LockoutEndDateUtc.Value.ToString();
                            var _ = _auditLogsBL.CreateAspNetUsersLockoutEndDateUtcModifiedEventAsync(oldValue, newValue, userId);
                        }

                        // send email notif to user
                        var urlBuilder = new UriBuilder(Request.Url.AbsoluteUri) { Path = Url.Action("login", "account") };
                        string loginUrl = urlBuilder.ToString();

                        using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
                        {
                            string subject = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_ACCOUNTUNLOCK).ConfigureAwait(false);
                            string body = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_ACCOUNTUNLOCK).ConfigureAwait(false);
                            body = body.Replace("{username}", user.UserName)
                                       .Replace("{loginurl}", loginUrl);

                            // send email notification to user for the temporary password   
                            // fire and forget
                            // do not wait for sendmail to finish
                            var mailerInstance = MailerInstance();
                            var _ = mailerInstance.SendMailAsync("ACCOUNT UNLOCK", User.Identity.Name, subject, body, new[] { user.Email });
                        }
                        return Json(new { Status = "SUCCESS", Message = $"Account has been unlocked successfully." });
                    }
                }

                string msg = "";
                foreach (var err in result.Errors)
                {
                    msg += err + "\n";
                }
                return Json(new { Status = "ERROR", Message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERROR", Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> LogOff(string q)
        {
            string userId = User.Identity?.GetUserId();
            await LogOffSessionAsync(userId);
            return RedirectToAction("Login", "Account");
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            string userId = User.Identity?.GetUserId();
            await LogOffSessionAsync(userId);
            return RedirectToAction("Login", "Account");
        }

        [SkipLogActionFilter]
        public ActionResult Logout()
        {
            return View();
        }

        [HttpGet]
        [SkipLogActionFilter]
        public async Task ForceSignout()
        {
            string userId = User.Identity?.GetUserId();
            await LogOffSessionAsync(userId);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SignoutUser(string userId)
        {
            try
            {
                var result = await UserManager.UpdateSecurityStampAsync(userId).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    string username = User.Identity.Name;
                    LogUserAttempt( $"Force sign out user ID: {userId}");
                    return Json(new { Status = "SUCCESS", Message = "User successfully signed out of the system." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var errList = new StringBuilder();
                    foreach (var err in result.Errors)
                    {
                        errList.Append(err + "\r\n");
                    }
                    return Json(new { Status = "ERROR", Message = errList.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var message = "Something went wrong.";
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, User.Identity.Name);

                return Json(new { Status = "ERROR", Message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SignoutAllUsers()
        {
            var m = MethodBase.GetCurrentMethod();
            string mName = m == null ? "-" : m.ReflectedType == null ? "--" : m.ReflectedType.FullName;

            try
            {
                int successCount = 0;
                var errors = new List<string>();
                var users = UserManager.Users.ToList();
                foreach (var user in users)
                {
                    string username = user.UserName;
                    string userId = user.Id;
                    try
                    {
                        var result = await UserManager.UpdateSecurityStampAsync(userId).ConfigureAwait(false);
                        if (result.Succeeded)
                        {
                            successCount++;
                            LogUserAttempt($"Force sign out user ID: {userId}");
                        }
                        else
                        {
                            var errList = new StringBuilder();
                            foreach (var err in result.Errors)
                            {
                                errList.Append(err + "<br/>");
                            }
                            errors.Add($">Error signing out user '{username}': {errList.ToString()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = $">Unable to sign out user '{username}'.";
                        errors.Add(message);
                        var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);
                    }
                }

                if (errors.Count == 0)
                {
                    return Json(new { Status = "SUCCESS", Message = "All users were successfully signed out of the system including you." });
                }
                else
                {
                    string message = $"{successCount} out of {users.Count} user{(users.Count > 1 ? "s" : "")} {(successCount > 1 ? "have" : "has")} been successfully signed out of the system.<br/>" +
                                     $"But the system encountered some problem{(errors.Count > 1 ? "s" : "")}:<br/>";
                    errors.Insert(0, message);
                    return Json(new { Status = "ERROR", Message = string.Join("<br/>", errors.ToArray()) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var message = "Something went wrong.";
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, User.Identity.Name);

                return Json(new { Status = "ERROR", Message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync().ConfigureAwait(false);
            if (userId == null)
            {
                throw new NullReferenceException("Unknown user.");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId).ConfigureAwait(false);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                throw new NullReferenceException("Unknown provider.");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [HttpGet]
        [SkipLogActionFilter]
        public ActionResult KeepAuthAlive()
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_accountManagersBL != null)
                    _accountManagersBL = null;
            }

            base.Dispose(disposing);
        }
    }
}