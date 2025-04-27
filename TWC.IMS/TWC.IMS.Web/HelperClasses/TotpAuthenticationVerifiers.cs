using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TWC.IMS.Web.Models;
using OtpSharp;
using Base32;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Web.HelperClasses.Interfaces;

namespace TWC.IMS.Web.HelperClasses
{
    public class GoogleAuthenticationVerifier : ITotpAuthenticationVerifier
    {
        public async Task<bool> VerifyAsync(string secretKey, string code, ApplicationUser user, ApplicationUserManager userManager, TwoFactorAuthType twoFactorAuthType)
        {
            long timeStepMatched = 0;
            byte[] securityKey = Base32Encoder.Decode(secretKey);
            var otp = new Totp(securityKey);
            if (otp.VerifyTotp(code, out timeStepMatched, new VerificationWindow(1, 1)))
            {
                // inform user via email
                var _ = TotpCodeVerifier.Send2FAEmailNotifAsync(user.UserName, user.Email, twoFactorAuthType).ConfigureAwait(false);
                user.IsGoogleAuthenticatorEnabled = true;
                user.GoogleAuthenticatorSecretKey = secretKey;
                await userManager.UpdateAsync(user).ConfigureAwait(false);
                return true;
            }
            else
                return false;
        }
    }

    public class MicrosoftAuthenticationVerifier : ITotpAuthenticationVerifier
    {
        public async Task<bool> VerifyAsync(string secretKey, string code, ApplicationUser user, ApplicationUserManager userManager, TwoFactorAuthType twoFactorAuthType)
        {
            long timeStepMatched = 0;
            byte[] securityKey = Base32Encoder.Decode(secretKey);
            var otp = new Totp(securityKey);
            if (otp.VerifyTotp(code, out timeStepMatched, new VerificationWindow(1, 1)))
            {
                // inform user via email
                var _ = TotpCodeVerifier.Send2FAEmailNotifAsync(user.UserName, user.Email, twoFactorAuthType).ConfigureAwait(false);
                user.IsMicrosoftAuthenticatorEnabled = true;
                user.MicrosoftAuthenticatorSecretKey = secretKey;
                await userManager.UpdateAsync(user).ConfigureAwait(false);
                return true;
            }
            else
                return false;
        }
    }
}