using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using TWC.IMS.Web.Models;
using System.Net.Mail;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.BL;
using System.Net.Http;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using System.Configuration;
using TWC.IMS.Common.HelperClasses;

namespace TWC.IMS.Web
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            if (message != null)
            {
                string to = message.Destination;
                string subject = message.Subject;
                string body = message.Body;
                string eventName = "2FA VIA EMAIL";

                var mailerInstance = TWC.IMS.Common.Mailer.Instance;
                //mailerInstance.ApplicationVersion = this.AppInstance.ApplicationVersion;
                //mailerInstance.Environment = this.AppInstance.Environment;
                //mailerInstance.ClientIPAddress = this.ClientIPAddress;
                //mailerInstance.IsMobileDevice = this.IsMobileDevice;
                //mailerInstance.UserAgent = this.UserAgent;
                //mailerInstance.UserRole = this.UserRole;
                return mailerInstance.SendMailAsync(eventName, to, subject, body, new[] { to });
            }
            return Task.FromResult(false);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public string Sid { get; set; }
        public string Username { get; set; }
        public bool IsMobileDevice { get; set; }
        public string UserRole { get; set; }
        public string UserAgent { get; set; }
        public string ClientIPAddress { get; set; }

        public async Task SendAsync(IdentityMessage message)
        {
            var smsInstance = TWC.IMS.Common.SmsService.Instance;
            smsInstance.ApplicationVersion = Application.Instance.ApplicationVersion;
            smsInstance.Environment = Application.Instance.Environment;
            smsInstance.IsMobileDevice = IsMobileDevice;
            smsInstance.UserRole = UserRole;
            smsInstance.UserAgent = UserAgent;
            smsInstance.ClientIPAddress = ClientIPAddress;

            this.Sid = await smsInstance.GenerateSmsOtpCodeAsync(Username, message.Destination, message.Body).ConfigureAwait(false);
        }

        public async Task SendPasswordChangeNotifAsync(ApplicationUser user, DateTime timestamp)
        {
            if (user != null && user.PhoneNumberConfirmed)
            {
                using (var scBL = new SystemConfigs(Username))
                {
                    string msgTemplate = await scBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.SMS_BODY_PASSWORD_CHANGE, false).ConfigureAwait(false);
                    if (msgTemplate != null)
                        msgTemplate = string.Format(msgTemplate, timestamp.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_LONG_1));
                    else
                        msgTemplate = string.Format("Your password has been changed successfully on {0}. If you do not recognize this change, contact your system administrator immediately.", timestamp.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_LONG_1));

                    IdentityMessage msg = new IdentityMessage
                    {
                        Destination = user.PhoneNumber,
                        Body = msgTemplate
                    };
                    var _ = this.SendAsync(msg);
                }
            }
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            bool allowOnlyAlphanumericUserNames = Convert.ToBoolean(ConfigurationManager.AppSettings["USER_VALIDATOR_ALLOW_ONLY_ALPHANUMERIC_USERNAMES"] ?? "false");
            bool requireUniqueEmail = Convert.ToBoolean(ConfigurationManager.AppSettings["USER_VALIDATOR_REQUIRE_UNIQUE_EMAIL"] ?? "true");

            int requiredLength = Convert.ToInt32(ConfigurationManager.AppSettings["PASSWORD_VALIDATOR_REQUIRED_LENGTH"] ?? "8");
            bool requireNonLetterOrDigit = Convert.ToBoolean(ConfigurationManager.AppSettings["PASSWORD_VALIDATOR_REQUIRE_NONLETTER_DIGIT"] ?? "true");
            bool requireDigit = Convert.ToBoolean(ConfigurationManager.AppSettings["PASSWORD_VALIDATOR_REQUIRE_DIGIT"] ?? "true");
            bool requireLowercase = Convert.ToBoolean(ConfigurationManager.AppSettings["PASSWORD_VALIDATOR_REQUIRE_LOWERCASE"] ?? "true");
            bool requireUppercase = Convert.ToBoolean(ConfigurationManager.AppSettings["PASSWORD_VALIDATOR_REQUIRE_UPPERCASE"] ?? "true");

            int maxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(ConfigurationManager.AppSettings["MAX_FAILEDACCESS_ATTEMPTS_BEFORE_LOCKOUT"] ?? "5");
            int defaultAccountLockoutTimeSpan = Convert.ToInt32(ConfigurationManager.AppSettings["DEFAULT_ACCOUNT_LOCKOUT_TIMESPAN_DAYS"] ?? "365");
            bool userLockoutEnabledByDefault = Convert.ToBoolean(ConfigurationManager.AppSettings["USER_LOCKOUT_ENABLED_BY_DEFAULT"] ?? "true");

            int tokenLifespan = Convert.ToInt32(ConfigurationManager.AppSettings["USER_TOKEN_PROVIDER_TOKEN_LIFESPAN_MINUTES"] ?? "1440"); // 1 day default

            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = allowOnlyAlphanumericUserNames,
                RequireUniqueEmail = requireUniqueEmail
            };

            // A2 Broken Authentication
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = requiredLength,
                RequireNonLetterOrDigit = requireNonLetterOrDigit,
                RequireDigit = requireDigit,
                RequireLowercase = requireLowercase,
                RequireUppercase = requireUppercase,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = userLockoutEnabledByDefault;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(defaultAccountLockoutTimeSpan);
            manager.MaxFailedAccessAttemptsBeforeLockout = maxFailedAccessAttemptsBeforeLockout;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            string otpMessage = ConfigurationManager.AppSettings["OTP_MESSAGE"] ?? "Your One-Time Passcode is {0}.";
            manager.RegisterTwoFactorProvider(TwoFactorAuthProvider.SMS.ToString(), new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = otpMessage
            });

            using (var scBL = new SystemConfigs(Environment.UserName))
            {
                string body = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => scBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_BODY_EMAIL_2FA_VERIFICATION));
                string subject = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => scBL.GetValueAsync(TWC.IMS.Models.HelperClasses.SystemConfigName.EMAIL_SUBJECT_EMAIL_2FA_VERIFICATION));

                manager.RegisterTwoFactorProvider(TwoFactorAuthProvider.EMAIL.ToString(), new EmailTokenProvider<ApplicationUser>
                {
                    Subject = subject,
                    BodyFormat = body
                });
            }
            manager.RegisterTwoFactorProvider(TwoFactorAuthProvider.GOOGLE_AUTH.ToString(), new GoogleAuthenticatorTokenProvider());
            manager.RegisterTwoFactorProvider(TwoFactorAuthProvider.MICROSOFT_AUTH.ToString(), new MicrosoftAuthenticatorTokenProvider());
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    TokenLifespan = TimeSpan.FromMinutes(tokenLifespan) // default: 1 day
                };
            }
            return manager;
        }

        public static void Test()
        {

        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole, string>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }
}