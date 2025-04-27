using TWC.IMS.Models.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Web.HelperClasses
{
    public class ReportHelpers
    {
        private static BL.SystemConfigs _systemConfigsBL = null;

        public static async Task GetReportExpirationAsync(HttpContextBase context)
        {
            string username = context.User.Identity.Name;
            _systemConfigsBL = new BL.SystemConfigs(username);
            var configValue = await _systemConfigsBL.GetValueAsync(SystemConfigName.REPORTS_EXPIRATION_DURATION_IN_DAYS).ConfigureAwait(false);
            context.Session["REPORT_EXPIRATION"] = configValue ?? "1";
        }
    }
}