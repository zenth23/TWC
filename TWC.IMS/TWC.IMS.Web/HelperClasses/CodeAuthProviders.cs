using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TWC.IMS.Web.Models;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Web.HelperClasses.Interfaces;

namespace TWC.IMS.Web.HelperClasses
{
    public class EmailCodeAuthProvider : ITwoFactorAuthProvider
    {
        public string Provider => "Email Code";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">Provide TotpBasedAuthenticatorViewModel.Code only</param>
        /// <param name="userManager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(TotpBasedAuthenticatorViewModel model, ApplicationUserManager userManager, ApplicationUser user)
        {
            var isValid = await userManager.TwoFactorProviders[Provider].ValidateAsync(Provider, model.Code, userManager, user).ConfigureAwait(false);
            if (isValid)
            {
                var result = await userManager.SetTwoFactorEnabledAsync(user.Id, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class PhoneCodeAuthProvider : ITwoFactorAuthProvider
    {
        public string Provider => "Phone Code";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">Provide TotpBasedAuthenticatorViewModel.Code only</param>
        /// <param name="userManager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(TotpBasedAuthenticatorViewModel model, ApplicationUserManager userManager, ApplicationUser user)
        {
            var isValid = await userManager.TwoFactorProviders[Provider].ValidateAsync(Provider, model.Code, userManager, user).ConfigureAwait(false);
            if (isValid)
            {
                var result = await userManager.SetTwoFactorEnabledAsync(user.Id, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class GoogleCodeAuthProvider : ITwoFactorAuthProvider
    {
        public TwoFactorAuthProvider Provider => TwoFactorAuthProvider.GOOGLE_AUTH;

        public async Task<bool> ValidateAsync(TotpBasedAuthenticatorViewModel model, ApplicationUserManager userManager, ApplicationUser user)
        {
            model.SecretKey = user.GoogleAuthenticatorSecretKey;
            
            TwoFactorAuthType twoFactorAuthType = TwoFactorAuthType.GOOGLE_DISABLED;
            var valid = await TotpCodeVerifier.VerifyTotpAsync(model, Provider, twoFactorAuthType, user, userManager).ConfigureAwait(false);
            if (valid)
            {
                var result = await userManager.SetTwoFactorEnabledAsync(user.Id, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class MicrosoftCodeAuthProvider : ITwoFactorAuthProvider
    {
        public TwoFactorAuthProvider Provider => TwoFactorAuthProvider.MICROSOFT_AUTH;

        public async Task<bool> ValidateAsync(TotpBasedAuthenticatorViewModel model, ApplicationUserManager userManager, ApplicationUser user)
        {
            model.SecretKey = user.MicrosoftAuthenticatorSecretKey;
            
            TwoFactorAuthType twoFactorAuthType = TwoFactorAuthType.MICROSOFT_DISABLED;
            var valid = await TotpCodeVerifier.VerifyTotpAsync(model, Provider, twoFactorAuthType, user, userManager).ConfigureAwait(false);
            if (valid)
            {
                var result = await userManager.SetTwoFactorEnabledAsync(user.Id, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}