using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.ChartModels
{
    public class AdminDashboardCounterModel
    {
        public int TotalErrors { get; set; }
        public int TotalLoggedUsers { get; set; }
        // top 4 only
        public IEnumerable<ErrorCountByMethodModel> ErrorCountByMethodList { get; set; }
    }

    public class ErrorCountByMethodModel
    {
        public string MethodName { get; set; }
        public int ErrorCount { get; set; }
    }

    public class ErrorFrequencyModel
    {
        public int ErrorCount { get; set; }
        public int InformationCount { get; set; }
        public int WarningCount { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ErrorRateModel
    {
        public int TotalErrorCount { get; set; }
        public int TotalLogCount { get; set; }
        public double Rate { get; set; }
        public int Month { get; set; }
        public string MonthString { get; set; }
    }

    // CVP - Current vs Previous
    public class ErrorRateCvpModel
    {
        public double RateCurrent { get; set; }
        public double RatePrevious { get; set; }
        public int Day { get; set; }
    }

    public class ErrorTrendModel
    {
        public int Hour { get; set; }
        public int ErrorCount { get; set; }
    }

    public class ErrorSeverityModel
    {
        public string ImpactLevel { get; set; }
        public int ErrorCount { get; set; }
    }

    public class UserRelatedMetricsModel
    {
        public string ProfileThumbnail { get; set; }
        public string Username { get; set; }
        public int ErrorCount { get; set; }
        public string IPAddress { get; set; }
        public string Role { get; set; }
        public string AppVersion { get; set; }
    }

    public class ApplicationVersionErrorDistributionModel
    {
        public int ErrorCount { get; set; }
        public string AppVersion { get; set; }
    }

    public class PageHitsModel
    {
        public string Username { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Activity { get; set; }
        public int HitCount { get; set; }
    }
}
