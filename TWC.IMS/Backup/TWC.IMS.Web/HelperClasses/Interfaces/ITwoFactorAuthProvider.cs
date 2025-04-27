

using TWC.IMS.Web.Models;
using System.Threading.Tasks;

namespace TWC.IMS.Web.HelperClasses.Interfaces
{
    public interface ITwoFactorAuthProvider
    {
        Task<bool> ValidateAsync(TotpBasedAuthenticatorViewModel model, ApplicationUserManager userManager, ApplicationUser user);
    }
}