using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TWC.IMS.Web.Models;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.BL;
using TWC.IMS.Models;
using System.Web.Security;
using System.Web.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using OtpSharp;
using Base32;
using TWC.IMS.Common.HelperClasses;
using System.Configuration;
using TWC.IMS.Web.HelperClasses.Interfaces;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class ManageController : BaseController
    {
        #region PRIVATE MEMBERS
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        private async Task<string> GetAvatarAsync()
        {
            string mimeType;
            string base64string;
            var base64data = (Dictionary<string, string>)TempData["AvatarImageBase64"];
            if (base64data != null && base64data.Count > 0)
            {
                var data = base64data.FirstOrDefault();
                mimeType = data.Key;
                base64string = data.Value;
            }
            else
            {
                // display previous avatar from database
                using (_userDetailsBL = new UserDetails(User.Identity.Name))
                {
                    var pd = await _userDetailsBL.GetByUsernameAsync(User.Identity.Name).ConfigureAwait(false);
                    if (pd != null && pd.Avatar != null)
                    {
                        mimeType = pd.AvatarMimeType;
                        using (MemoryStream stream = new MemoryStream(pd.Avatar))
                        {
                            base64string = await TWC.IMS.Common.Tools.ConvertImageStreamToBase64(stream).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        // else display default picture from file
                        var avatarPath = Server.MapPath("~/content/images/avatar-default.png");
                        using (var file = new FileStream(avatarPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            mimeType = System.Web.MimeMapping.GetMimeMapping(avatarPath);
                            base64string = await TWC.IMS.Common.Tools.ConvertImageStreamToBase64(file).ConfigureAwait(false);
                        }
                    }
                }
            }
            string b64s = $"data:{mimeType};base64,{base64string}";
            return b64s;
        }

        private TotpBasedAuthenticatorViewModel GenerateTotpCode()
        {
            byte[] secretKey = KeyGeneration.GenerateRandomKey(20);
            string appName = Application.Instance.ApplicationName;
            string username = $"{appName}: {User.Identity.GetUserName()}";
            string barcodeUrl = KeyUrl.GetTotpUrl(secretKey, username) + "&issuer=" + appName;
            return new TotpBasedAuthenticatorViewModel
            {
                AppName = appName,
                SecretKey = Base32Encoder.Encode(secretKey),
                BarcodeUrl = barcodeUrl
            };
        }

        private async Task<ActionResult> Disable2faAsync(TotpBasedAuthenticatorViewModel model, TwoFactorAuthProvider twoFactorAuthProvider, TwoFactorAuthType twoFactorAuthType)
        {
            string viewName = "DisableGoogleAuthenticator";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                var valid = await TotpCodeVerifier.VerifyTotpAsync(model, twoFactorAuthProvider, twoFactorAuthType, user, UserManager).ConfigureAwait(false);
                if (valid)
                {
                    if (twoFactorAuthProvider == TwoFactorAuthProvider.GOOGLE_AUTH)
                    {
                        user.IsGoogleAuthenticatorEnabled = false;
                        user.GoogleAuthenticatorSecretKey = null;
                    }
                    else
                    {
                        user.IsMicrosoftAuthenticatorEnabled = false;
                        user.MicrosoftAuthenticatorSecretKey = null;
                        viewName = "DisableMicrosoftAuthenticator";
                    }
                    // update AspNetUser
                    await UserManager.UpdateAsync(user);

                    // inform user via email
                    var _ = TotpCodeVerifier.Send2FAEmailNotifAsync(user.UserName, user.Email, twoFactorAuthType).ConfigureAwait(false);

                    TempData["SUCCESS_MESSAGE"] = "Google Authenticator successfully disabled.";

                    string userId = user.Id;
                    return RedirectToAction("AutoSignInUser", new { userId = userId, c = "Manage", a = "Profile", anchor = "#account" });
                }
                else
                {
                    TempData["ERROR_MESSAGE"] = "Invalid code.";
                    return View(viewName, model);
                }
            }
            else
                TempData["ERROR_MESSAGE"] = "Failed to disable Google Authenticator.";

            return Redirect("~/manage/profile#account");
        }

        private ITwoFactorAuthProvider GetAuthProviderInstance(TwoFactorAuthProvider provider)
        {
            switch (provider)
            {
                case TwoFactorAuthProvider.EMAIL:
                    return new EmailCodeAuthProvider();

                case TwoFactorAuthProvider.SMS:
                    return new PhoneCodeAuthProvider();

                case TwoFactorAuthProvider.GOOGLE_AUTH:
                    return new GoogleCodeAuthProvider();

                case TwoFactorAuthProvider.MICROSOFT_AUTH:
                    return new MicrosoftCodeAuthProvider();

                default:
                    return null;
            }
        }
        #endregion

        public new ActionResult Profile()
        {
            return View();
        }

        [SkipLogActionFilter]
        // not possible to return a task from a PartialView
        public ActionResult GetAccountPartialView()
        {
            var userId = User.Identity.GetUserId();
            using (_aspNetUsersBL = new AspNetUsers(User.Identity.Name))
            {
                var anu = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => _aspNetUsersBL.GetByUsernameAsync(User.Identity.Name));
                if (anu != null)
                {
                    var userDetail = anu.UserDetails.FirstOrDefault();
                    if (userDetail != null)
                    {
                        var model = new AccountViewModel();
                        model.UserId = userId;
                        model.UserName = anu.UserName;
                        model.UniqueKey = userDetail.UniqueKey;

                        var role = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => UserManager.GetRolesAsync(userId));
                        var roleName = role.FirstOrDefault();
                        model.RoleName = roleName;

                        var user = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => UserManager.FindByIdAsync(userId));
                        model.AccountSetting = new IndexViewModel
                        {
                            HasPassword = HasPassword(),
                            PhoneNumber = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => UserManager.GetPhoneNumberAsync(userId)),
                            TwoFactor = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => UserManager.GetTwoFactorEnabledAsync(userId)),
                            //Logins = await UserManager.GetLoginsAsync(userId),
                            //BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                            IsGoogleAuthenticatorEnabled = user?.IsGoogleAuthenticatorEnabled ?? false,
                            IsMicrosoftAuthenticatorEnabled = user?.IsMicrosoftAuthenticatorEnabled ?? false,
                            IsEmailVerified = user?.EmailConfirmed ?? false,
                            IsPhoneVerified = user?.PhoneNumberConfirmed ?? false,
                        };

                        var roleObj = RoleManager.FindByName(roleName);
                        model.User_Role = roleObj?.Id;
                        return PartialView("_Account", model);
                    }
                    else
                        ModelState.AddModelError("", "Personal details not found.");
                }
                else
                    ModelState.AddModelError("", "Unknown user data.");

                return PartialView("_Account");
            }
        }

        [SkipLogActionFilter]
        // not possible to return a task from a PartialView
        public ActionResult GetContactPartialView()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            if (user != null)
            {
                var model = new ContactViewModel();
                model.UserId = userId;
                model.Email = user.Email;
                model.IsEmailVerified = user.EmailConfirmed;
                model.PhoneNumber = user.PhoneNumber;
                model.IsPhoneVerified = user.PhoneNumberConfirmed;

                return PartialView("_Contact", model);
            }
            else
                ModelState.AddModelError("", "Unknown user data.");

            return PartialView("_Contact");
        }

        [SkipLogActionFilter]
        // not possible to return a task from a PartialView
        public ActionResult GetPersonalPartialView()
        {
            var userId = User.Identity.GetUserId();
            using (_aspNetUsersBL = new AspNetUsers(User.Identity.Name))
            {
                var anu = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => _aspNetUsersBL.GetByUsernameAsync(User.Identity.Name));
                if (anu != null)
                {
                    var UserDetail = anu.UserDetails.FirstOrDefault();
                    if (UserDetail != null)
                    {
                        var model = new PersonalViewModel();
                        model.Id = UserDetail.Id;
                        model.Avatar = UserDetail.Avatar;
                        model.EmployeeId = UserDetail.EmployeeId;
                        model.FirstName = UserDetail.FirstName;
                        model.LastName = UserDetail.LastName;
                        model.MiddleName = UserDetail.MiddleName;
                        model.Suffix = UserDetail.Suffix;
                        model.Nickname = UserDetail.Nickname;
                        model.UserDetail_AspNetUser = userId;
                        model.UserDetailRowVersion = UserDetail.RowVersion;

                        return PartialView("_Personal", model);
                    }
                    else
                        ModelState.AddModelError("", "Personal details not found.");
                }
                else
                    ModelState.AddModelError("", "Unknown user data.");
            }

            return PartialView("_Personal");
        }

        [HttpPost, ActionName("Profile")]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "ACCOUNT")]
        public async Task<ActionResult> SaveAccountDetails(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // change password
                string currentPassword = model.ChangePasswordModel.OldPassword;
                string newPassword = model.ChangePasswordModel.NewPassword;
                // verify old password
                var user = await UserManager.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
                if (user != null)
                {
                    bool isValidPassword = await UserManager.CheckPasswordAsync(user, model.ChangePasswordModel.OldPassword).ConfigureAwait(false);
                    if (isValidPassword)
                    {
                        // verify new password
                        using (_accountManagersBL = new AccountManagers(User.Identity.Name))
                        {
                            bool isInUse = await _accountManagersBL.IsPasswordInUseAsync(model.UserId, newPassword).ConfigureAwait(false);
                            if (!isInUse)
                            {
                                // then validate and change password
                                var result = await UserManager.ChangePasswordAsync(model.UserId, currentPassword, newPassword).ConfigureAwait(false);
                                if (result.Succeeded)
                                {
                                    // get timestamp before the other processes
                                    DateTime timestamp = DateTime.Now;

                                    // requrey user for the updated password
                                    user = await UserManager.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
                                    if (user != null)
                                    {
                                        // log new password
                                        var phObj = new PasswordHistory();
                                        phObj.IsTemporaryPassword = false;
                                        phObj.PasswordHash = user.PasswordHash;
                                        phObj.PasswordHistory_AspNetUser = model.UserId;

                                        using (var phBL = new PasswordHistories(User.Identity.Name))
                                        {
                                            await phBL.InsertAsync(phObj).ConfigureAwait(false);
                                        }

                                        // tell identity to force logout this user
                                        await UserManager.UpdateSecurityStampAsync(model.UserId).ConfigureAwait(false);

                                        // logout user
                                        LogOffSession(model.UserId);

                                        // send text msg about the change
                                        var smsService = ((SmsService)UserManager.SmsService);
                                        smsService.Username = User.Identity.Name;
                                        smsService.IsMobileDevice = Request.Browser.IsMobileDevice;
                                        smsService.UserRole = Session["USERROLES"]?.ToString();
                                        var _ = smsService.SendPasswordChangeNotifAsync(user, timestamp);

                                        // redirect user
                                        TempData["SUCCESS_MESSAGE"] = "Password successfully changed.";
                                        return RedirectToAction("login", "account");
                                    }
                                    else
                                        ModelState.AddModelError("", $"Unknown user '{User.Identity.Name}'.");
                                }
                                else
                                {
                                    foreach (var err in result.Errors)
                                    {
                                        ModelState.AddModelError("", err);
                                    }
                                }
                            }
                            else
                                ModelState.AddModelError("", "Password already in use.");
                        }
                    }
                    else
                    {
                        TempData["ERROR_MESSAGE"] = "Invalid current password.";
                        return Redirect("~/manage/profile#account");
                    }
                }
                else
                    ModelState.AddModelError("", $"Unknown user '{User.Identity.Name}'.");
            }
            return View("Profile", model);
        }

        [HttpPost, ActionName("Profile")]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "CONTACT")]
        public async Task<ActionResult> SaveContactDetails(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = await User.Identity.GetUserIdAsync().ConfigureAwait(false);
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                if (user != null)
                {
                    // if email has been changed
                    if (string.Compare(user.Email, model.Email, true) != 0)
                        user.EmailConfirmed = false; // unverify

                    // if phone has been changed
                    if (string.Compare(user.PhoneNumber, model.PhoneNumber, true) != 0)
                        user.PhoneNumberConfirmed = false; // unverify

                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    IdentityResult updateResult = await UserManager.UpdateAsync(user).ConfigureAwait(false);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var err in updateResult.Errors)
                        {
                            ModelState.AddModelError("", err);
                        }
                    }

                    if (!updateResult.Errors.Any())
                    {
                        TempData["SUCCESS_MESSAGE"] = "Contact details successfully saved.";
                        return Redirect("~/manage/profile#contact");
                    }
                }
                else
                    ModelState.AddModelError("", "Unknown user.");
            }
            return View("Profile", model);
        }

        [HttpPost, ActionName("Profile")]
        [ValidateAntiForgeryToken]
        [MultipleSubmit(Name = "ACTION", Argument = "PERSONAL")]
        public async Task<ActionResult> SaveUserDetails(PersonalViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (_userDetailsBL = new UserDetails(User.Identity.Name))
                {
                    var obj = await _userDetailsBL.GetByUsernameAsync(User.Identity.Name).ConfigureAwait(false);
                    if (obj != null)
                    {
                        obj.EmployeeId = model.EmployeeId;
                        obj.FirstName = model.FirstName;
                        obj.LastName = model.LastName;
                        obj.MiddleName = model.MiddleName;
                        obj.Suffix = model.Suffix;
                        obj.Nickname = model.Nickname;
                        obj.UserDetail_AspNetUser = model.UserDetail_AspNetUser;
                        obj.RowVersion = model.UserDetailRowVersion;

                        var avatarDataFinal = new KeyValuePair<string, byte[]>();
                        if (TempData["AvatarImage"] != null)
                        {
                            var avatarByteData = (Dictionary<string, Dictionary<string, byte[]>>)TempData["AvatarImage"];
                            var avatar = avatarByteData.FirstOrDefault();
                            var avatarData = avatar.Value;
                            avatarDataFinal = avatarData.FirstOrDefault();
                            obj.AvatarMimeType = avatarDataFinal.Key;
                            obj.Avatar = avatarDataFinal.Value;
                        }
                        await _userDetailsBL.UpdateAsync(obj).ConfigureAwait(false);

                        TempData["SUCCESS_MESSAGE"] = "Details successfully saved.";

                        // do some cleanup
                        TempData["AvatarImage"] = null;
                        avatarDataFinal = default(KeyValuePair<string, byte[]>);

                        // then redirect
                        return Redirect("~/manage/profile#personal");
                    }
                    else
                        ModelState.AddModelError("", "Can't find user data.");
                }
            }
            return View("Profile", model);
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> AvatarUpload_Save(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                var file = files.FirstOrDefault();
                // check file if valid
                string[] allowedMimes = new[] { "image/png", "image/jpeg" };
                string contentType = file.ContentType.ToLower();
                bool isValid = allowedMimes.Any(a => string.Compare(a, contentType, true) == 0);
                if (isValid)
                {
                    // check file size if valid (500KB)
                    int allowedFileSize = 512000;
                    var fileSize = file.ContentLength;
                    if (fileSize <= allowedFileSize)
                    {
                        var filename = Path.GetFileName(file.FileName).ToLower();

                        string base64string = await TWC.IMS.Common.Tools.ConvertImageStreamToBase64(file.InputStream).ConfigureAwait(false);
                        var base64Data = new Dictionary<string, string>();
                        base64Data.Add(contentType, base64string);
                        TempData["AvatarImageBase64"] = base64Data;

                        // convert file to bytes
                        byte[] avatarBytes;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream ms = new MemoryStream();
                            inputStream.Position = 0;
                            inputStream.CopyTo(ms);
                            avatarBytes = ms.ToArray();
                        }

                        var avatarData = new Dictionary<string, Dictionary<string, byte[]>>();
                        var data = new Dictionary<string, byte[]>();
                        data.Add(contentType, avatarBytes);
                        avatarData.Add(filename, data);
                        // save image file bytes to TempData
                        TempData["AvatarImage"] = avatarData;

                        // return emty to signify success
                        return Content("");
                    }
                    else
                        return Content("File is too large.");
                }
                else
                    return Content("Invalid file.");
            }
            return Content("No file to upload.");
        }

        [SkipLogActionFilter]
        public ActionResult AvatarUpload_Remove(string fileNames)
        {
            if (!string.IsNullOrWhiteSpace(fileNames))
            {
                var fname = Path.GetFileName(fileNames).ToLower();
                var data = (Dictionary<string, Dictionary<string, byte[]>>)TempData["AvatarImage"];
                data.Remove(fname);
                TempData["AvatarImage"] = data; //re-set
                // return emty to signify success
                return Content("");
            }
            return Content("No file to remove.");
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> GetAvatarBase64String()
        {
            string b64s = await GetAvatarAsync().ConfigureAwait(false);
            return Json(b64s, JsonRequestBehavior.AllowGet);
        }

        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey)).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number).ConfigureAwait(false);
            if (UserManager.SmsService != null)
            {
                string otpMessage = ConfigurationManager.AppSettings["OTP_MESSAGE"] ?? "Your One-Time Passcode is {0}.";
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = string.Format(otpMessage, code)
                };
                await UserManager.SmsService.SendAsync(message).ConfigureAwait(false);
            }
            return RedirectToAction("VerifyPhoneNumber", new { pn = TWC.IMS.Common.Cryptography.Base64Encode(model.Number) });
        }

        public async Task<ActionResult> SendCode()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                throw new NullReferenceException("Unknown user.");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId).ConfigureAwait(false);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie).ConfigureAwait(false);
            if (result != null && result.Identity != null && !String.IsNullOrEmpty(result.Identity.GetUserId()))
            {
                string userId = result.Identity.GetUserId();
                // for sms and email only. will return null for totp (totp app should generate the token)
                var token = await UserManager.GenerateTwoFactorTokenAsync(userId, model.SelectedProvider).ConfigureAwait(false);
                // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
                await UserManager.NotifyTwoFactorTokenAsync(userId, model.SelectedProvider, token).ConfigureAwait(false);
                return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider });
            }
            else
            {
                throw new NullReferenceException("Unknown provider.");
            }
        }

        public ActionResult VerifyCode(string provider)
        {
            return View(new TotpBasedAuthenticatorViewModel { Provider = provider });
        }

        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            string userId = User.Identity.GetUserId();
            var result = await UserManager.SetTwoFactorEnabledAsync(userId, true).ConfigureAwait(false);
            if (result.Succeeded)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                if (user != null)
                {
                    TempData["SUCCESS_MESSAGE"] = "Two-Factor Authentication has been enabled.";
                    // auto-login
                    return RedirectToAction("AutoSignInUser", new { userId = userId, c = "Manage", a = "Profile", anchor = "#account" });
                }
                else
                {
                    TempData["ERROR_MESSAGE"] = "Failed to locate your account.";
                    LogOffSession();
                    return RedirectToAction("login", "account");
                }
            }
            else
                TempData["ERROR_MESSAGE"] = "Failed to enable Two-Factor Authentication.";

            return RedirectToAction("Profile", "Manage");
        }

        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication(TotpBasedAuthenticatorViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                string userId = user.Id;
                TwoFactorAuthProvider provider;
                if (Enum.TryParse(model.Provider, out provider))
                {
                    ITwoFactorAuthProvider instance = GetAuthProviderInstance(provider);
                    if (instance != null)
                    {
                        var isValid = await instance.ValidateAsync(model, UserManager, user).ConfigureAwait(false);
                        if (isValid)
                        {
                            var result = await UserManager.SetTwoFactorEnabledAsync(userId, false).ConfigureAwait(false);
                            if (result.Succeeded)
                            {
                                TempData["SUCCESS_MESSAGE"] = "Two-Factor Authentication has been disabled.";
                                return RedirectToAction("AutoSignInUser", new { userId = userId, c = "Manage", a = "Profile", anchor = "#account" });
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["ERROR_MESSAGE"] = "Failed to locate your account.";
                LogOffSession();
                return RedirectToAction("login", "account");
            }

            TempData["ERROR_MESSAGE"] = "Failed to disable Two-Factor Authentication.";
            return RedirectToAction("Profile", "Manage");
        }

        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string pn)
        {
            if (pn == null)
            {
                TempData["ERROR_MESSAGE"] = "Invalid phone number.";
                return Redirect("~/manage/profile#contact");
            }
            else
            {
                pn = await TWC.IMS.Common.Cryptography.Base64Decode(pn).ConfigureAwait(false);

                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), pn).ConfigureAwait(false);

                // Send an SMS through the SMS provider to verify the phone number
                // text message is automatically generated by MITTO API
                var smsService = ((SmsService)UserManager.SmsService);
                smsService.Username = User.Identity.Name;
                smsService.IsMobileDevice = Request.Browser.IsMobileDevice;
                smsService.UserRole = Session["USERROLES"]?.ToString();

                //await UserManager.SendSmsAsync(User.Identity.GetUserId(), code).ConfigureAwait(false);

                string otpMessage = ConfigurationManager.AppSettings["OTP_MESSAGE"] ?? "Your One-Time Passcode is {0}.";
                IdentityMessage message = new IdentityMessage
                {
                    Body = string.Format(otpMessage, code),
                    Destination = pn
                };
                await UserManager.SmsService.SendAsync(message).ConfigureAwait(false);

                string sid = ((SmsService)UserManager.SmsService).Sid;
                return View(new VerifyPhoneNumberViewModel { PhoneNumber = pn, Sid = sid });
            }
        }

        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string userId = User.Identity.GetUserId();

            // validate code
            string sid = model.Sid;
            bool isValid = await UserManager.VerifyChangePhoneNumberTokenAsync(userId, model.Code, model.PhoneNumber).ConfigureAwait(false);
            if (isValid)
            {
                // re-login user
                ApplicationUser user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                if (user != null)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    user.PhoneNumberConfirmed = true;
                    var updateResult = await UserManager.UpdateAsync(user).ConfigureAwait(false);
                    if (updateResult.Succeeded)
                    {
                        TempData["SUCCESS_MESSAGE"] = "Phone number successfully verified.";
                        return Redirect("~/manage/profile#contact");
                    }
                    else
                    {
                        foreach (var err in updateResult.Errors)
                        {
                            ModelState.AddModelError("", err);
                        }
                    }
                }
            }
            else
                ModelState.AddModelError("", "Invalid code.");

            return View(model);
        }

        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId()).ConfigureAwait(false);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false).ConfigureAwait(false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //GET: /Manage/ChangePassword
        [AllowAnonymous]
        public ActionResult ChangePassword(string code, string userId)
        {
            var model = new ChangePasswordViewModel();
            model.Code = code;
            model.UserId = userId;

            if (code == null)
                throw new NullReferenceException("Unknown code.");
            else
                return View(model);
        }

        // POST: 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
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

            // check for current password
            var isEqual = await UserManager.CheckPasswordAsync(user, model.OldPassword.Trim()).ConfigureAwait(false);
            if (isEqual)
            {
                using (_accountManagersBL = new AccountManagers(user.UserName))
                {
                    bool isInUse = await _accountManagersBL.IsPasswordInUseAsync(model.UserId, model.NewPassword).ConfigureAwait(false);
                    if (!isInUse)
                    {
                        var result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword.Trim(), model.NewPassword.Trim()).ConfigureAwait(false);
                        if (result.Succeeded)
                        {
                            var _ = LogPasswordAsync(model.NewPassword.Trim(), user.Id, user.UserName);

                            // tell identity to force logout this user
                            await UserManager.UpdateSecurityStampAsync(model.UserId).ConfigureAwait(false);

                            return RedirectToAction("ResetPasswordConfirmation", "Account");
                        }
                        AddErrors(result);
                    }
                    else
                        ModelState.AddModelError("", "Password already in use. Please try another one.");
                }
            }
            else
                ModelState.AddModelError("", $"Invalid current password.");

            return View(model);
        }

        #region GOOGLE AUTHENTICATOR

        public async Task<ActionResult> DisableGoogleAuthenticator()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                var model = new TotpBasedAuthenticatorViewModel
                {
                    SecretKey = user.GoogleAuthenticatorSecretKey
                };
                return View(model);
            }
            else
                TempData["ERROR_MESSAGE"] = "Failed to disable Google Authenticator.";

            return Redirect("~/manage/profile#account");
        }

        [HttpPost, ActionName("DisableGoogleAuthenticator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableGoogleAuthenticatorValidate(TotpBasedAuthenticatorViewModel model)
        {
            return await Disable2faAsync(model, TwoFactorAuthProvider.GOOGLE_AUTH, TwoFactorAuthType.GOOGLE_DISABLED).ConfigureAwait(false);
        }

        public ActionResult EnableGoogleAuthenticator()
        {
            var model = GenerateTotpCode();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableGoogleAuthenticatorValidate(TotpBasedAuthenticatorViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                var valid = await TotpCodeVerifier.VerifyTotpAsync(model, TwoFactorAuthProvider.GOOGLE_AUTH, TwoFactorAuthType.GOOGLE_ENABLED, user, UserManager).ConfigureAwait(false);
                if (valid)
                    TempData["SUCCESS_MESSAGE"] = "Microsoft Authenticator successfully enabled.";
                else
                    return View("EnableGoogleAuthenticator", model);
            }
            else
                TempData["ERROR_MESSAGE"] = "Failed to disable Google Authenticator.";

            return Redirect("~/manage/profile#account");
        }
        #endregion

        #region MICROSOFT AUTHENTICATOR

        public async Task<ActionResult> DisableMicrosoftAuthenticator()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                var model = new TotpBasedAuthenticatorViewModel
                {
                    SecretKey = user.MicrosoftAuthenticatorSecretKey
                };
                return View(model);
            }
            else
                TempData["ERROR_MESSAGE"] = "Failed to disable Microsoft Authenticator.";

            return Redirect("~/manage/profile#account");
        }

        [HttpPost, ActionName("DisableMicrosoftAuthenticator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableMicrosoftAuthenticatorValidate(TotpBasedAuthenticatorViewModel model)
        {
            return await Disable2faAsync(model, TwoFactorAuthProvider.MICROSOFT_AUTH, TwoFactorAuthType.MICROSOFT_DISABLED).ConfigureAwait(false);
        }

        public ActionResult EnableMicrosoftAuthenticator()
        {
            var model = GenerateTotpCode();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableMicrosoftAuthenticatorValidate(TotpBasedAuthenticatorViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                var valid = await TotpCodeVerifier.VerifyTotpAsync(model, TwoFactorAuthProvider.MICROSOFT_AUTH, TwoFactorAuthType.MICROSOFT_ENABLED, user, UserManager).ConfigureAwait(false);
                if (valid)
                    TempData["SUCCESS_MESSAGE"] = "Microsoft Authenticator successfully enabled.";
                else
                    return Redirect("~/manage/enablegoogleauthenticator");
            }
            else
                TempData["ERROR_MESSAGE"] = "Failed to disable Google Authenticator.";

            return Redirect("~/manage/profile#account");
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyEmailAddress()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                var _ = TotpCodeVerifier.Send2FAEmailNotifAsync(user.UserName, user.Email, TwoFactorAuthType.VERIFY_EMAIL, callbackUrl);
            }
            return Content("Please check your email.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }
    }
}