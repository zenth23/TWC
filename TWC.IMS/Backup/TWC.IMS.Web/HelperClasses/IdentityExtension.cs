using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace TWC.IMS.Web.HelperClasses
{
    public static class IdentityExtension
    {
        public static async Task<string> GetUserIdAsync(this IIdentity identity)
        {
            string username = identity.Name;
            // get user id from database
            using (var anuBL = new BL.AspNetUsers(username))
            {
                var obj = await anuBL.GetByUsernameAsync(username).ConfigureAwait(false);
                if (obj != null)
                    return obj.Id;
                else
                    throw new NullReferenceException($"Unknown username '{username}'.");
            }
        }

        public static async Task<string> GetFirstNameAsync(this IIdentity identity)
        {
            string username = identity.Name;
            // get user id from database
            using (var udBL = new BL.UserDetails(username))
            {
                var obj = await udBL.GetByUsernameAsync(username).ConfigureAwait(false);
                if (obj != null)
                    return obj.FirstName;
                else
                    return "Anonymous";
            }
        }
    }
}