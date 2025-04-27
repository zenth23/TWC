using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Web.HelperClasses.Interfaces
{
    public interface ITotpAuthenticationVerifier
    {
        Task<bool> VerifyAsync(string securityKey, string code, ApplicationUser user, ApplicationUserManager userManager, TwoFactorAuthType twoFactorAuthType);
    }
}
