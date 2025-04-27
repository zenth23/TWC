using Base32;
using OtpSharp;
using TWC.IMS.Common.HelperClasses;
using TWC.IMS.Web.HelperClasses.Interfaces;
using TWC.IMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TWC.IMS.Web.HelperClasses
{
    public class TotpCodeVerifier
    {
        private static ITotpAuthenticationVerifier GetTotpVerifierInstance(TwoFactorAuthProvider twoFactorAuthProvider)
        {
            switch (twoFactorAuthProvider)
            {
                case TwoFactorAuthProvider.GOOGLE_AUTH:
                    return new GoogleAuthenticationVerifier();

                case TwoFactorAuthProvider.MICROSOFT_AUTH:
                    return new MicrosoftAuthenticationVerifier();

                default:
                    return null;
            }
        }

        public static async Task<bool> VerifyTotpAsync(TotpBasedAuthenticatorViewModel model, TwoFactorAuthProvider twoFactorAuthProvider, TwoFactorAuthType twoFactorAuthType, ApplicationUser user, ApplicationUserManager userManager)
        {
            byte[] secretKey = Base32Encoder.Decode(model.SecretKey);
            ITotpAuthenticationVerifier verifier = GetTotpVerifierInstance(twoFactorAuthProvider);
            if (verifier != null)
            {
                return await verifier.VerifyAsync(model.SecretKey, model.Code, user, userManager, twoFactorAuthType).ConfigureAwait(false);
            }
            else
            {
                return false;
            }
        }

        public static async Task Send2FAEmailNotifAsync(string username, string userEmailAddress, TwoFactorAuthType authType, string callbackUrl = null)
        {
            string subject = "";
            string body = "";
            string eventName = "";

            using (var systemConfigsBL = new BL.SystemConfigs(username))
            {
                switch (authType)
                {
                    case TwoFactorAuthType.GOOGLE_ENABLED:
                        eventName = "EnableGoogleAuthenticator";
                        subject = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_GOOGLE_2FA_ENABLED).ConfigureAwait(false);
                        body = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_GOOGLE_2FA_ENABLED).ConfigureAwait(false);
                        break;

                    case TwoFactorAuthType.GOOGLE_DISABLED:
                        eventName = "DisableGoogleAuthenticator";
                        subject = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_GOOGLE_2FA_DISABLED).ConfigureAwait(false);
                        body = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_GOOGLE_2FA_DISABLED).ConfigureAwait(false);
                        break;

                    case TwoFactorAuthType.MICROSOFT_ENABLED:
                        eventName = "EnableMicrosoftAuthenticator";
                        subject = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_MICROSOFT_2FA_ENABLED).ConfigureAwait(false);
                        body = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_MICROSOFT_2FA_ENABLED).ConfigureAwait(false);
                        break;

                    case TwoFactorAuthType.MICROSOFT_DISABLED:
                        eventName = "DisableMicrosoftAuthenticator";
                        subject = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_MICROSOFT_2FA_DISABLED).ConfigureAwait(false);
                        body = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_MICROSOFT_2FA_DISABLED).ConfigureAwait(false);
                        break;

                    case TwoFactorAuthType.VERIFY_EMAIL:
                        eventName = "VerifyEmailAddress";
                        subject = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_EMAILCONFIRMATION).ConfigureAwait(false);
                        body = await systemConfigsBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_EMAILCONFIRMATION).ConfigureAwait(false);
                        break;
                }
            }

            body = body.Replace("{username}", username);
            body = body.Replace("{callbackUrl}", callbackUrl ?? "");

            var _ = TWC.IMS.Common.Mailer.Instance.SendMailAsync(eventName, username, subject, body, new[] { userEmailAddress });
        }
    }
}