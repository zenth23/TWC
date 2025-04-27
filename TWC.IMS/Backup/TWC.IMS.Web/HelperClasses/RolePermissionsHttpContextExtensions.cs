using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace TWC.IMS.Web.HelperClasses
{
    public static class RolePermissionsHttpContextExtensions
    {
        private static string _permissionsSessionName = "ROLE_PERMISSIONS";
        private static string _userRoles = "USERROLES";

        #region PRIVATE MEMBERS
        private static IEnumerable<string> GetPermissionsListFromSession(HttpContextBase httpContextBase)
        {
            return (IEnumerable<string>)httpContextBase.Session[_permissionsSessionName];
        }

        private static async Task AddItemAsync(HttpContextBase httpContextBase)
        {
            if (GetPermissionsListFromSession(httpContextBase) == null)
            {
                var caa = new CustomAuthorizeAttribute();
                var result = await caa.AuthorizedModulesAsync(httpContextBase).ConfigureAwait(false);
                httpContextBase.Session[_permissionsSessionName] = result.Value;
                httpContextBase.Session[_userRoles] = result.Key;
            }
        }
        #endregion

        public static async Task<IEnumerable<string>> GetPermissionsAsync(this HttpContextBase httpContextBase)
        {
            await AddItemAsync(httpContextBase).ConfigureAwait(false);
            return GetPermissionsListFromSession(httpContextBase);
        }

        public static bool HasPermission(this HttpContextBase httpContextBase, string accessName)
        {
            return TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => HasPermissionAsync(httpContextBase, accessName));
        }

        public static async Task<bool> HasPermissionAsync(this HttpContextBase httpContextBase, string accessName)
        {
            await GetPermissionsAsync(httpContextBase).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(accessName)) return false;

            return ((IEnumerable<string>)httpContextBase.Session[_permissionsSessionName])
                                                        .Any(a => string.Compare(a, accessName, true) == 0);
        }

        public static bool IsSmitsAdminUsername(this HttpContextBase httpContextBase, string currentUsername)
        {
            var tmpUsers = Application.Instance.SmitsAdminUsername;// httpContextBase.Cache["SMITS_ADMIN_USERNAME"];
            if (tmpUsers != null)
            {
                var users = tmpUsers.ToString().Split(',');
                return users.Any(a => string.Compare(a, currentUsername, true) == 0);
            }
            return false;
        }
    }
}