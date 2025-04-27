using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using TWC.IMS.BL;
using TWC.IMS.Models;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Common;
using TWC.IMS.Common.HelperClasses;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class UserMaintenanceController : BaseController
    {
        #region PRIVATE MEMBERS

        private string _origRoleName = "";

        private async Task<ActionResult> DisplayRoleDetailsAsync(string key)
        {
            // check if we need to sign out all users on permission change
            using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
            {
                ViewBag.IsAutoLogoutEnabled = await _systemConfigsBL.IsAutoLogoutEnabledAsync().ConfigureAwait(false);
            }

            await IsCurrentUserAdminAsync().ConfigureAwait(false);

            var roleModelState = (ModelStateSummary[])TempData["ModelState"];
            var roleViewModel = (RoleViewModel)TempData["ViewModel"];
            if (roleViewModel != null)
            {
                // from redirect due to model error
                foreach (var err in roleModelState)
                {
                    if (err.ErrorMessages.Length > 0)
                    {
                        foreach (var innerErr in err.ErrorMessages)
                        {
                            ModelState.AddModelError(err.PropertyName, innerErr);
                        }
                    }
                }
                return View("role", roleViewModel);
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                bool isAuthorizedRole = await CanAddAsync("Roles").ConfigureAwait(false);
                if (!isAuthorizedRole)
                    throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);
                
                return View("role");
            }
            else
            {
                bool isAuthorizedRole = await CanEditAsync("Roles").ConfigureAwait(false);
                if (!isAuthorizedRole)
                    throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);
                
                var role = await RoleManager.FindByIdAsync(key.Trim()).ConfigureAwait(false);
                if (role != null)
                {
                    var obj = new RoleViewModel();
                    obj.Id = role.Id;
                    obj.Name = role.Name;
                    obj.Users = role.Users;

                    using (_roleDetailsBL = new RoleDetails(User.Identity.Name))
                    {
                        var pd = await _roleDetailsBL.GetAsync(role.Id).ConfigureAwait(false);
                        if (pd != null)
                        {
                            obj.IsActive = pd.IsActive;
                            obj.IsAdmin = pd.IsAdmin;
                            obj.Description = pd.Description;
                            obj.CreatedBy = string.Format("{0} {1}", pd.CreatedBy, (pd.Created == null ? "" : "on " + pd.Created.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                            obj.ModifiedBy = string.Format("{0} {1}", pd.ModifiedBy, (pd.Modified == null ? "" : "on " + pd.Modified.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                        }
                        return View("Role", obj);
                    }
                }
                else
                    throw new HttpException(404, $"Record '{key}' not found");
            }
        }

        private async Task<ActionResult> DisplayUserDetailsAsync(string key)
        {
            var userModelState = (ModelStateSummary[])TempData["ModelState"];
            var userViewModel = (UserViewModel)TempData["ViewModel"];
            if (userViewModel != null)
            {
                // from redirect due to model error
                foreach (var err in userModelState)
                {
                    if (err.ErrorMessages.Length > 0)
                    {
                        foreach (var innerErr in err.ErrorMessages)
                        {
                            ModelState.AddModelError(err.PropertyName, innerErr);
                        }
                    }
                }

                return View("user", userViewModel);
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                bool isAuthorized = await CanAddAsync("Users").ConfigureAwait(false);
                if (!isAuthorized)
                    throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);

                // CREATE NEW
                var model = new UserViewModel();
                return View("User", model);
            }
            else
            {
                bool isAuthorized = await CanEditAsync("Users").ConfigureAwait(false);
                if (!isAuthorized)
                    throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);
                
                // UPDATE HERE
                // from grid list redirect
                using (_userDetailsBL = new UserDetails(User.Identity.Name))
                {
                    var guid = Guid.Parse(key);
                    var user = await _userDetailsBL.GetAsync(guid).ConfigureAwait(false);
                    if (user != null)
                    {
                        var objUVM = new UserViewModel();
                        objUVM.Id = user.Id;
                        objUVM.IsActive = user.IsActive;
                        objUVM.LastLoginDate = user.LastLoginDatetime;
                        objUVM.ActivationDate = user.ActivationDatetime;
                        objUVM.DeactivationDate = user.DeactivationDatetime;
                        objUVM.ExpirationDate = user.ExpirationDatetime;

                        objUVM.CreatedBy = string.Format("{0} {1}", user.CreatedBy, (user.Created == null ? "" : "on " + user.Created.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                        objUVM.ModifiedBy = string.Format("{0} {1}", user.ModifiedBy, (user.Modified == null ? "" : "on " + user.Modified.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));

                        objUVM.PersonalModel.EmployeeId = user.EmployeeId;
                        objUVM.PersonalModel.FirstName = user.FirstName;
                        objUVM.PersonalModel.LastName = user.LastName;
                        objUVM.PersonalModel.MiddleName = user.MiddleName;
                        objUVM.PersonalModel.Suffix = user.Suffix;
                        objUVM.PersonalModel.Nickname = user.Nickname;
                        objUVM.PersonalModel.UserDetail_AspNetUser = user.UserDetail_AspNetUser;
                        objUVM.PersonalModel.UserDetailRowVersion = user.RowVersion;

                        objUVM.ContactModel.Email = user.AspNetUser.Email;
                        objUVM.AccountModel.UserId = user.UserDetail_AspNetUser;
                        objUVM.AccountModel.UserName = user.AspNetUser.UserName;

                        bool isAdmin = false;
                        var roles = await UserManager.GetRolesAsync(user.UserDetail_AspNetUser).ConfigureAwait(false);
                        var roleName = roles.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(roleName))
                        {
                            var role = await RoleManager.FindByNameAsync(roleName).ConfigureAwait(false);
                            if (role != null)
                            {
                                objUVM.User_Role = role.Id;
                                var rolesList = await GetRoleWithDetailsAsync().ConfigureAwait(false);
                                var rd = rolesList.Where(a => string.Compare(a.Name, roleName, true) == 0).FirstOrDefault();
                                isAdmin = rd == null ? false : rd.IsAdmin;
                                objUVM.AccountModel.IsAdmin = isAdmin;
                            }
                            else
                                throw new NullReferenceException($"'{roleName}' role not found.");
                        }
                        ViewBag.IsAdmin = isAdmin;

                        return View("User", objUVM);
                    }
                    else
                        throw new NullReferenceException($"Unknown user with Unique Key '{key}'.");
                }
            }
        }

        private async Task<IEnumerable<RoleUserViewModel>> GetUsersByRoleAsync(string roleId)
        {
            var list = new List<RoleUserViewModel>();
            var role = await RoleManager.FindByIdAsync(roleId).ConfigureAwait(false);
            if (role != null)
            {
                using (_userDetailsBL = new UserDetails(User.Identity.Name))
                using (_aspNetUsersBL = new AspNetUsers(User.Identity.Name))
                {
                    foreach (var user in role.Users)
                    {
                        string userId = user.UserId;
                        var u = await _aspNetUsersBL.GetAsync(userId).ConfigureAwait(false);
                        if (u != null)
                        {
                            var ud = u.UserDetails.FirstOrDefault();
                            if (ud != null)
                            {
                                list.Add(new RoleUserViewModel()
                                {
                                    RoleId = roleId,
                                    RoleName = role.Name,
                                    UserId = userId,
                                    UserName = u.UserName,
                                    EmployeeId = ud.EmployeeId,
                                    FirstName = ud.FirstName,
                                    LastName = ud.LastName,
                                    MiddleName = ud.MiddleName,
                                    Suffix = ud.Suffix,
                                    Nickname = ud.Nickname,
                                    Email = u.Email
                                });
                            }
                        }
                    }
                }
            }
            return list;
        }

        private async Task<bool> SignoutUsersInRoleAsync(string roleId)
        {
            var users = (await GetUsersByRoleAsync(roleId).ConfigureAwait(false)).ToList();
            // if current user is included, current user must be the last one to be logged out of the system
            string userId = await User.Identity.GetUserIdAsync().ConfigureAwait(false);
            var ruvmObj = users.FirstOrDefault(a => string.Compare(a.UserId, userId, true) == 0);
            if (ruvmObj != null)
            {
                users.Remove(ruvmObj);
                users.Insert(users.Count, ruvmObj);
            }

            foreach (var uId in users.Select(a => a.UserId).Distinct())
            {
                await UserManager.UpdateSecurityStampAsync(uId).ConfigureAwait(false);
            }

            Session.Clear();
            Session.Abandon();

            return ruvmObj != null;
        }

        private async Task SendEmailNotifToUserAsync(string firstName, string lastName, string emailAddress, string username, string tempPassword)
        {
            var urlBuilder = new UriBuilder(Request.Url.AbsoluteUri) { Path = Url.Action("login", "account", null) };
            string loginUrl = urlBuilder.ToString();

            string subject = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_NEWACCOUNT).ConfigureAwait(false);
            string body = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_NEWACCOUNT).ConfigureAwait(false);
            //string fullname = $"{model.PersonalModel.FirstName} {model.PersonalModel.MiddleName} {model.PersonalModel.LastName}";
            string fullname = $"{firstName} {lastName}";
            body = body.Replace("{fullname}", fullname)
                       .Replace("{username}", username)
                       .Replace("{password}", tempPassword)
                       .Replace("{loginurl}", loginUrl);

            // send email notification to user for the temporary password   
            // fire and forget
            // do not wait for sendmail to finish
            var mailerInstance = TWC.IMS.Common.Mailer.Instance;
            mailerInstance.ApplicationVersion = this.AppInstance.ApplicationVersion;
            mailerInstance.Environment = this.AppInstance.Environment;
            mailerInstance.ClientIPAddress = this.ClientIPAddress;
            mailerInstance.IsMobileDevice = this.IsMobileDevice;
            mailerInstance.UserAgent = this.UserAgent;
            mailerInstance.UserRole = this.UserRole;
            var _ = mailerInstance.SendMailAsync("NEW ACCOUNT", User.Identity.Name, subject, body, new[] { emailAddress });
        }

        private async Task<int> InsertNewUserAsync(UserViewModel model, string userId)
        {
            var obj = new UserDetail();
            obj.EmployeeId = model.PersonalModel.EmployeeId.Trim();
            obj.FirstName = model.PersonalModel.FirstName.Trim();
            obj.LastName = model.PersonalModel.LastName.Trim();
            obj.MiddleName = model.PersonalModel.MiddleName?.Trim();
            obj.Suffix = model.PersonalModel.Suffix?.Trim();
            obj.Nickname = model.PersonalModel.Nickname?.Trim();
            obj.UserDetail_AspNetUser = userId;
            obj.IsActive = model.IsActive;

            DateTime? activationDatetime = null;
            if (model.IsActive)
                activationDatetime = DateTime.Now;

            obj.ActivationDatetime = activationDatetime;
            obj.DeactivationDatetime = null;

            var today = DateTime.Now;
            bool isExpired = model.ExpirationDate < today;
            bool isLocked = await UserManager.IsLockedOutAsync(userId).ConfigureAwait(false);
            obj.Status = _userDetailsBL.SetUserStatus(isLocked, isExpired, obj.IsActive);

            return await _userDetailsBL.InsertAsync(obj).ConfigureAwait(false);
        }

        private async Task AddUserToRoleAsync(string userId, string username, string rolename)
        {
            var result = await UserManager.AddToRoleAsync(userId, rolename.Trim()).ConfigureAwait(false);
            foreach (var err in result.Errors)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, new Exception($"Error adding user '{username}' to role '{rolename}'."), User.Identity.Name);
            }
        }

        private async Task<string> GenerateTemporaryPasswordAsync()
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

        private async Task UpdateUserAsync(UserViewModel model)
        {
            var pdObj = new UserDetail();
            pdObj.Id = model.Id;
            pdObj.EmployeeId = model.PersonalModel.EmployeeId.Trim();
            pdObj.FirstName = model.PersonalModel.FirstName.Trim();
            pdObj.LastName = model.PersonalModel.LastName.Trim();
            pdObj.MiddleName = model.PersonalModel.MiddleName?.Trim();
            pdObj.Suffix = model.PersonalModel.Suffix?.Trim();
            pdObj.Nickname = model.PersonalModel.Nickname?.Trim();
            pdObj.IsActive = model.IsActive;
            pdObj.RowVersion = model.PersonalModel.UserDetailRowVersion;
            pdObj.UserDetail_AspNetUser = model.AccountModel.UserId;
            pdObj.ExpirationDatetime = model.ExpirationDate;

            var today = DateTime.Now;
            bool isExpired = model.ExpirationDate < today;
            bool isLocked = await UserManager.IsLockedOutAsync(model.AccountModel.UserId).ConfigureAwait(false);
            pdObj.Status = _userDetailsBL.SetUserStatus(isLocked, isExpired, pdObj.IsActive);

            await _userDetailsBL.UpdateAsync(pdObj).ConfigureAwait(false);
        }

        private async Task LogAspNetUserRolesAuditAsync(string userId, string oldRoleName, string rolename)
        {
            // audit the change manually
            var alObj = new AuditLog();
            alObj.TableName = "AspNetUserRoles";
            alObj.ColumnName = "Role";
            alObj.RowID = userId;
            alObj.EventType = AuditLogEventType.MODIFIED.ToString();
            alObj.OldValue = oldRoleName;
            alObj.NewValue = rolename.Trim();

            using (_auditLogsBL = new AuditLogs(User.Identity.Name))
            {
                await _auditLogsBL.InsertAsync(alObj).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns list of object { Id, Name }
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<dynamic>> GetRoleWithDetailsAsync(bool? isActive = null)
        {
            using (_roleDetailsBL = new RoleDetails(User.Identity.Name))
            {
                var roleDetails = await _roleDetailsBL.GetListAsync(isActive).ConfigureAwait(false);
                var roles = RoleManager.Roles.ToList();
                var list = (from r in roles
                            join d in roleDetails on r.Id equals d.RoleDetail_AspNetRole into rd
                            from rdres in rd.DefaultIfEmpty()
                            select new
                            {
                                r.Id,
                                r.Name,
                                Description = rdres?.Description,
                                IsAdmin = rdres?.IsAdmin ?? false,
                                IsActive = rdres?.IsActive ?? false
                            });

                if (isActive.HasValue)
                    list = list.Where(a => a.IsActive);

                return list.OrderBy(a => a.Name)
                           .ToList();
            }
        }

        private async Task<ActionResult> InsertNewRoleAsync(string roleName, RoleViewModel model)
        {
            // check if role name exists
            var role = await RoleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (role != null)
            {
                ModelState.AddModelError("", $"Role '{roleName}' already exists.");
                return null;
            }

            role = new IdentityRole(roleName);
            var result = await RoleManager.CreateAsync(role).ConfigureAwait(false);
            if (result.Succeeded)
            {
                string roleId = role.Id;
                // at the same time, created RoleDetails as well
                var rdObj = new RoleDetail();
                rdObj.IsAdmin = model.IsAdmin;
                rdObj.Description = model.Description;
                rdObj.RoleDetail_AspNetRole = roleId;
                await _roleDetailsBL.InsertAsync(rdObj).ConfigureAwait(false);

                var accesses = model.Accesses.Where(a => a.IsChecked).Select(a => new { a.Id, a.Name });
                foreach (var item in accesses)
                {
                    string name = item.Name;
                    int mid = item.Id; // ModuleId
                    var m = await _modulesBL.GetAsync(mid).ConfigureAwait(false);
                    if (m != null)
                    {
                        var ma = await _moduleAccessesBL.GetAsync(mid, name).ConfigureAwait(false);
                        if (ma != null)
                        {
                            int maId = ma.Id;
                            await _rolePermissionsBL.InsertAsync(new TWC.IMS.Models.RolePermission()
                            {
                                RolePermission_ModuleAccess = maId,
                                RolePermission_Role = roleId
                            }).ConfigureAwait(false);
                        }
                    }
                }
                return Redirect("~/maintenance#roles");
            }
            else
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", $"{err}");
                }
                return null;
            }
        }

        private async Task<ActionResult> UpdateRoleAsync(string roleName, RoleViewModel model)
        {
            var role = await RoleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (role != null)
            {
                string roleId = role.Id;
                var rdObj = await _roleDetailsBL.GetAsync(roleId).ConfigureAwait(false);
                rdObj.Description = model.Description;
                rdObj.IsAdmin = model.IsAdmin;
                rdObj.IsActive = model.IsActive;
                await _roleDetailsBL.UpdateAsync(rdObj).ConfigureAwait(false);

                // update AspNetRole here when RoleDetails saving is successful
                await RoleManager.UpdateAsync(role).ConfigureAwait(false);

                // perform delete-insert in RolePermissions
                await _rolePermissionsBL.DeleteByRoleAsync(roleId).ConfigureAwait(false);

                // save permissions
                await SaveAccessesAsync(roleId, model).ConfigureAwait(false);

                // check if we need to sign out all users on permission change
                bool isIncluded = false;
                bool isEnabled = await _systemConfigsBL.IsAutoLogoutEnabledAsync().ConfigureAwait(false);
                if (isEnabled)
                    isIncluded = await SignoutUsersInRoleAsync(roleId).ConfigureAwait(false);

                if (isIncluded)
                    return RedirectToAction("Login", "Account");
                else
                    return Redirect("~/maintenance#roles");
            }
            else
                ModelState.AddModelError("", $"Role '{roleName}' already exists.");

            return null;
        }

        private async Task SaveAccessesAsync(string roleId, RoleViewModel model)
        {
            var accesses = model.Accesses.Where(a => a.IsChecked).Select(a => new { a.Id, a.Name });
            foreach (var item in accesses)
            {
                string name = item.Name;
                int mid = item.Id; // ModuleId
                var m = await _modulesBL.GetAsync(mid).ConfigureAwait(false);
                if (m != null)
                {
                    var ma = await _moduleAccessesBL.GetAsync(mid, name).ConfigureAwait(false);
                    if (ma != null)
                    {
                        int maId = ma.Id;
                        await _rolePermissionsBL.InsertAsync(new TWC.IMS.Models.RolePermission()
                        {
                            RolePermission_ModuleAccess = maId,
                            RolePermission_Role = roleId
                        }).ConfigureAwait(false);
                    }
                }
            }
        }

        #endregion

        // GET: UserMaintenance
        public async Task<ActionResult> Index()
        {
            bool isAuthorized = await CanViewAsync("Users").ConfigureAwait(false);
            isAuthorized = await CanViewAsync("Roles").ConfigureAwait(false);

            return View();
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> ReadUsers([DataSourceRequest]DataSourceRequest request)
        {
            using (_userDetailsBL = new UserDetails(User.Identity.Name))
            {
                var users = await _userDetailsBL.GetListAsync().ConfigureAwait(false);
                var list = new List<UserViewModel>();
                foreach (var item in users)
                {
                    var roles = await UserManager.GetRolesAsync(item.AspNetUser.Id).ConfigureAwait(false);
                    var model = new UserViewModel();
                    model.Id = item.Id;
                    model.UniqueKey = item.UniqueKey;
                    model.PersonalModel.EmployeeId = item.EmployeeId;
                    model.PersonalModel.FirstName = item.FirstName;
                    model.PersonalModel.LastName = item.LastName;
                    model.PersonalModel.MiddleName = item.MiddleName;
                    model.PersonalModel.Suffix = item.Suffix;
                    model.PersonalModel.Nickname = item.Nickname;
                    model.Created = item.Created?.DateTime.AsNullable();
                    model.CreatedBy = item.CreatedBy;
                    model.Modified = item.Modified?.DateTime.AsNullable();
                    model.ModifiedBy = item.ModifiedBy;
                    model.ContactModel.Email = item.AspNetUser.Email;
                    model.AccountModel.UserId = item.AspNetUser.Id;
                    model.AccountModel.UserName = item.AspNetUser.UserName;
                    model.AccountModel.RoleName = string.IsNullOrWhiteSpace(roles.FirstOrDefault()) ? "[unassigned]" : roles.FirstOrDefault();
                    model.AccountModel.Status = item.Status;

                    model.ActivationDate = item.ActivationDatetime;
                    model.DeactivationDate = item.DeactivationDatetime;
                    model.ExpirationDate = item.ExpirationDatetime;

                    list.Add(model);
                }

                DataSourceResult result = list.OrderByDescending(a => a.Modified ?? a.Created).ToDataSourceResult(request);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// for grids
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SkipLogActionFilter]
        public async Task<ActionResult> ReadRoles([DataSourceRequest]DataSourceRequest request)
        {
            var list = new List<RoleViewModel>();
            using (_aspNetRolesBL = new AspNetRoles(User.Identity.Name))
            {
                var roles = await _aspNetRolesBL.GetListAsync().ConfigureAwait(false);
                foreach (var item in roles)
                {
                    var rd = item.RoleDetails.FirstOrDefault();
                    if (rd != null)
                    {
                        list.Add(new RoleViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = rd.Description,
                            IsAdmin = rd.IsAdmin,
                            IsActive = rd.IsActive,
                            Created = rd.Created == null ? null : rd.Created.Value.DateTime.AsNullable(),
                            CreatedBy = rd.CreatedBy,
                            Modified = rd.Modified == null ? null : rd.Modified.Value.DateTime.AsNullable(),
                            ModifiedBy = rd.ModifiedBy
                        });
                    }
                }
            }
            DataSourceResult result = list.OrderByDescending(a => a.Modified ?? a.Created).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// For comboboxes
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        [SkipLogActionFilter]
        public async Task<JsonResult> GetRolesList(string searchKey = "")
        {
            var list = await GetRoleWithDetailsAsync(true).ConfigureAwait(false);
            list = list.Where(a => a.Name.ToLower().Contains(searchKey.ToLower()));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // GET: UserMaintenance/Details/5
        public async Task<ActionResult> Details(string type, string key)
        {
            ViewBag.Referrer = Request.UrlReferrer;
            ViewBag.Mode = string.IsNullOrWhiteSpace(key) ? "CREATE" : "EDIT";

            switch (type.Trim().ToUpper())
            {
                case "U":   // - USER
                    return await DisplayUserDetailsAsync(key).ConfigureAwait(false);
                case "R":    // - ROLE 
                    return await DisplayRoleDetailsAsync(key).ConfigureAwait(false);
                default:
                    throw new HttpException(404, "Page not found.");
            }
        }

        // POST: UserMaintenance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailsRole(RoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (_rolePermissionsBL = new RolePermissions(User.Identity.Name))
                    using (_modulesBL = new Modules(User.Identity.Name))
                    using (_moduleAccessesBL = new ModuleAccesses(User.Identity.Name))
                    using (_roleDetailsBL = new RoleDetails(User.Identity.Name))
                    using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
                    using (_aspNetUsersBL = new AspNetUsers(User.Identity.Name))
                    {
                        string roleName = model.Name;
                        // INSERT NEW 
                        if (model.Id == null)
                        {
                            bool isAuthorized = await CanAddAsync("Roles").ConfigureAwait(false);
                            if (!isAuthorized)
                                throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);
                            
                            var result = await InsertNewRoleAsync(roleName, model).ConfigureAwait(false);
                            if (result != null)
                                return result;
                        }
                        else
                        {
                            bool isAuthorized = await CanEditAsync("Roles").ConfigureAwait(false);
                            if (!isAuthorized)
                                throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);
                            
                            var result = await UpdateRoleAsync(roleName, model).ConfigureAwait(false);
                            if (result != null)
                                return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            var list = ModelState.Select(state => new ModelStateSummary
            {
                PropertyName = state.Key,
                ErrorMessages = state.Value.Errors.Select(b => b.ErrorMessage).ToArray()
            }).ToArray();

            TempData["ModelState"] = list;
            TempData["ViewModel"] = model;
            return RedirectToAction("details", new { type = "r", name = _origRoleName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailsUser(UserViewModel model)
        {
            string origUsername = "";
            try
            {
                if (ModelState.IsValid)
                {
                    using (_aspNetUsersBL = new AspNetUsers(User.Identity.Name))
                    using (_userDetailsBL = new UserDetails(User.Identity.Name))
                    using (_systemConfigsBL = new SystemConfigs(User.Identity.Name))
                    using (_passwordHistoriesBL = new PasswordHistories(User.Identity.Name))
                    {
                        string rolename = Request.Form["User_Role_input"];

                        var rolesList = await GetRoleWithDetailsAsync().ConfigureAwait(false);
                        var rolesWithDetails = rolesList.Where(a => string.Compare(a.Name, rolename, true) == 0);
                        var roleAssigned = rolesWithDetails.FirstOrDefault();
                        bool isAdmin = roleAssigned.IsAdmin;

                        if (model.Id == 0)
                        {
                            bool isAuthorized = await CanAddAsync("Users").ConfigureAwait(false);
                            if (!isAuthorized)
                                throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);

                            #region INSERT
                            // validate email address
                            var isEmailValid = await TWC.IMS.Common.Tools.IsValidEmailAddress(model.ContactModel.Email).ConfigureAwait(false);
                            if (!isEmailValid)
                            {
                                ModelState.AddModelError("", "Invalid email address format.");

                                var list3 = ModelState.Select(state => new ModelStateSummary
                                {
                                    PropertyName = state.Key,
                                    ErrorMessages = state.Value.Errors.Select(b => b.ErrorMessage).ToArray()
                                }).ToArray();

                                TempData["ModelState"] = list3;
                                TempData["ViewModel"] = model;
                                return RedirectToAction("details", new { type = "u", name = origUsername });
                            }

                            string username = model.AccountModel.UserName;//await TWC.IMS.Common.StandardUsernameGenerator.GenerateUsernameAsync(model.PersonalModel.FirstName, model.PersonalModel.LastName, model.PersonalModel.MiddleName, model.PersonalModel.Suffix, model.PersonalModel.Nickname).ConfigureAwait(false);
                            model.AccountModel.UserName = username;
                            

                            var user = new ApplicationUser();
                            user.Email = model.ContactModel.Email.Trim();
                            user.UserName = username;

                            string tempPassword = await _systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.TEMP_PASS).ConfigureAwait(false);//await GenerateTemporaryPasswordAsync().ConfigureAwait(false);
                            var userObj = await UserManager.CreateAsync(user, tempPassword).ConfigureAwait(false);
                            if (userObj.Succeeded)
                            {
                                var u = await UserManager.FindByNameAsync(user.UserName).ConfigureAwait(false);
                                if (u != null)
                                {
                                    string userId = u.Id;
                                    await InsertNewUserAsync(model, userId).ConfigureAwait(false);

                                    // add user to role
                                    await AddUserToRoleAsync(userId, username, rolename).ConfigureAwait(false);

                                    // log password
                                    await LogPasswordAsync(tempPassword, userId, u.UserName, true).ConfigureAwait(false);

                                    // send mail notif
                                    var _ = this.SendEmailNotifToUserAsync(model.PersonalModel.FirstName, model.PersonalModel.LastName, model.ContactModel.Email, user.UserName, tempPassword).ConfigureAwait(false);

                                    return RedirectToAction("Index");
                                }
                                else
                                    ModelState.AddModelError("", $"User '{user.UserName}' not found.");
                            }
                            else
                            {
                                foreach (var err in userObj.Errors)
                                {
                                    ModelState.AddModelError("", err);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            bool isAuthorized = await CanEditAsync("Users").ConfigureAwait(false);
                            if (!isAuthorized)
                                throw new UnauthorizedAccessException(TWC.IMS.Common.Messages.NOT_AUTHORIZED);
                            
                            #region UPDATE
                            var u = await UserManager.FindByIdAsync(model.AccountModel.UserId).ConfigureAwait(false);
                            if (u != null)
                            {
                                bool isEmailConfirmed = u.EmailConfirmed;
                                string username = u.UserName;
                                origUsername = username;

                                // validate email address
                                var isEmailValid = await TWC.IMS.Common.Tools.IsValidEmailAddress(model.ContactModel.Email);
                                if (!isEmailValid)
                                {
                                    ModelState.AddModelError("Email", "Invalid email address format.");

                                    var list3 = ModelState.Select(state => new ModelStateSummary
                                    {
                                        PropertyName = state.Key,
                                        ErrorMessages = state.Value.Errors.Select(b => b.ErrorMessage).ToArray()
                                    }).ToArray();

                                    TempData["ModelState"] = list3;
                                    TempData["ViewModel"] = model;
                                    return RedirectToAction("details", new { type = "u", name = origUsername });
                                }

                                string origEmail = u.Email;
                                string origPhoneNumber = u.PhoneNumber;

                                var roles = await UserManager.GetRolesAsync(model.AccountModel.UserId).ConfigureAwait(false);
                                string oldRoleName = roleAssigned.Name;

                                await UpdateUserAsync(model).ConfigureAwait(false);

                                // remove from assigned roles
                                await UserManager.RemoveFromRolesAsync(model.AccountModel.UserId, roles.ToArray()).ConfigureAwait(false);
                                // then add to new role
                                var result = await UserManager.AddToRoleAsync(model.AccountModel.UserId, rolename.Trim()).ConfigureAwait(false);
                                if (result.Succeeded)
                                {
                                    if (string.Compare(oldRoleName, rolename.Trim()) != 0)
                                    {
                                        string userId = model.AccountModel.UserId;
                                        var _ = LogAspNetUserRolesAuditAsync(userId, oldRoleName, rolename).ConfigureAwait(false);
                                    }

                                    // if not the same
                                    if (string.Compare(origEmail, model.ContactModel.Email, true) != 0)
                                    {
                                        try
                                        {
                                            await _aspNetUsersBL.UpdateUserEmailAsync(model.AccountModel.UserId, model.ContactModel.Email).ConfigureAwait(false);
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("", $"Error updating email: {ex.InnerException}");
                                        }
                                    }

                                    // if no error
                                    if (ModelState.Values.Count(a => a.Errors.Count != 0) == 0)
                                    {
                                        if (string.Compare(User.Identity.Name, origUsername.Trim(), true) == 0)
                                        {
                                            // redirect to an action that implements [Authorize] attribute than
                                            // to account/login since it implements [AllowAnonymous] attribute
                                            // [AllowAnonymous] attribute does not validate authorization
                                            return RedirectToActionPermanent("logout", "account", new { ReturnUrl = "" });
                                        }

                                        return RedirectToAction("Index");
                                    }
                                }
                                else
                                {
                                    var m = MethodBase.GetCurrentMethod();
                                    string mName = m == null ? "-" : m.ReflectedType == null ? "--" : m.ReflectedType.FullName;
                                    foreach (var err in result.Errors)
                                    {
                                        string msg = $"Error adding user '{username}' to role '{rolename}': {err}";
                                        ModelState.AddModelError("", msg);
                                        
                                        var _ = this.LogErrorAsync(MessageType.ERROR, new Exception(msg), User.Identity.Name);
                                    }
                                }
                            }
                            else
                                ModelState.AddModelError("", $"Unknown user '{model.AccountModel.UserName}'.");
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            var list = ModelState.Select(state => new ModelStateSummary
            {
                PropertyName = state.Key,
                ErrorMessages = state.Value.Errors.Select(b => b.ErrorMessage).ToArray()
            }).ToArray();

            TempData["ModelState"] = list;
            TempData["ViewModel"] = model;
            return RedirectToAction("details", new { type = "u", name = origUsername });
        }

        // POST: UserMaintenance/Delete/5
        [CustomAuthorize(AccessName = "Users.CanDelete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(string key, string username)
        {
            try
            {
                // check if user exists
                var user = await UserManager.FindByNameAsync(username).ConfigureAwait(false);
                if (user != null)
                {
                    // delete Personal Details first
                    using (_userDetailsBL = new UserDetails(User.Identity.Name))
                    {
                        Guid uniqueKey = Guid.Empty;
                        bool isValid = Guid.TryParse(key, out uniqueKey);
                        if (isValid)
                        {
                            await _userDetailsBL.DeleteAsync(uniqueKey).ConfigureAwait(false);

                            var result = await UserManager.DeleteAsync(user).ConfigureAwait(false);
                            if (result.Succeeded)
                            {
                                return Json(new { Status = "SUCCESS", Message = "Record successfully deleted." });
                            }
                            else
                            {
                                var error = result.Errors.FirstOrDefault();
                                return Json(new { Status = "ERROR", Message = error });
                            }
                        }
                        else
                            return Json(new { Status = "ERROR", Message = "Invalid key." });
                    }
                }
                else
                    return Json(new { Status = "ERROR", Message = $"Username '{username}' not found." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERROR", Message = ex.Message });
            }
        }

        // POST: UserMaintenance/Delete/5
        [CustomAuthorize(AccessName = "Roles.CanDelete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRole(string id, string name)
        {
            try
            {
                // check if role exists
                var role = await RoleManager.FindByNameAsync(name).ConfigureAwait(false);
                if (role != null)
                {
                    // check for assigned users
                    int usersCount = role.Users.Count;
                    if (usersCount == 0)
                    {
                        // delete role
                        var result = await RoleManager.DeleteAsync(role).ConfigureAwait(false);
                        // just in case
                        // MANDATORY: signout all users under that role
                        var _ = SignoutUsersInRoleAsync(id);
                        return Json(new { Status = "SUCCESS", Message = "Record successfully deleted." });
                    }
                    else
                        return Json(new { Status = "ERROR", Message = $"There{(usersCount > 1 ? " are users " : "'s a user ")}assigned to this role. Cannot proceed." });
                }
                else
                    return Json(new { Status = "ERROR", Message = $"Role '{name}' not found." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERROR", Message = ex.Message });
            }
        }

        [SkipLogActionFilter]
        public async Task<ActionResult> ReadRoleUsers([DataSourceRequest]DataSourceRequest request, string roleid)
        {
            var list = await GetUsersByRoleAsync(roleid).ConfigureAwait(false);
            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [SkipLogActionFilter]
        public PartialViewResult RoleUsers(string roleid)
        {
            var list = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => GetUsersByRoleAsync(roleid));
            return PartialView("_RoleUsers", list);
        }

        [SkipLogActionFilter]
        public PartialViewResult AssignedAccounts(int userid)
        {
            using (_userDetailsBL = new UserDetails(User.Identity.Name))
            {
                var user = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => _userDetailsBL.GetAsync(userid));
                if (user != null)
                {

                }
                return PartialView("_AssignedAccounts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveUserFromRole(string uid, string role)
        {
            try
            {
                var result = await UserManager.RemoveFromRoleAsync(uid, role).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    // MANDATORY: logout user
                    // use this instead of UserManager.UpdateSecurityStampAsync()
                    using (_aspNetUsersBL = new AspNetUsers(User.Identity.Name))
                    {
                        var e = _aspNetUsersBL.UpdateSecurityStampAsync(uid).ConfigureAwait(false);
                    }
                    return Json(new { Status = "SUCCESS", Message = "User successfully removed from role." });
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

        [HttpPost]
        [SkipLogActionFilter]
        public Task UserAgreedOnAssignmentDeletionDueToRoleChange()
        {
            TempData["USER_AGREED_ON_ASSIGNMENT_DELETE"] = true;
            return Task.FromResult(0);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager != null)
                {
                    UserManager.Dispose();
                }

                if (RoleManager != null)
                {
                    RoleManager.Dispose();
                }

                if (_rolePermissionsBL != null)
                    _rolePermissionsBL = null;

                if (_modulesBL != null)
                    _modulesBL = null;

                if (_moduleAccessesBL != null)
                    _moduleAccessesBL = null;

                if (_aspNetUsersBL != null)
                    _aspNetUsersBL = null;

                if (_userDetailsBL != null)
                    _userDetailsBL = null;

                if (_systemConfigsBL != null)
                    _systemConfigsBL = null;

                if (_aspNetRolesBL != null)
                    _aspNetRolesBL = null;

                if (_roleDetailsBL != null)
                    _roleDetailsBL = null;

                if (_passwordHistoriesBL != null)
                    _passwordHistoriesBL = null;
            }

            base.Dispose(disposing);
        }
    }
}