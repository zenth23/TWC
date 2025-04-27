using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.ChartModels
{
    public class TotalSalesReportModel
    {
        [Display(Name = "License Currency Code")]
        public string LicenseCurrencyCode { get; set; }
        [Display(Name = "Previous Year")]
        public int PreviousYear { get; set; }
        [Display(Name = "Current Year")]
        public int CurrentYear { get; set; }

        [Display(Name = "Total Amount Previous Year")]
        public decimal TotalAmountPreviousYear { get; set; }
        [Display(Name = "Total Amount Current Year")]
        public decimal TotalAmountCurrentYear { get; set; }
        public decimal Percentage { get; set; }
    }

    public class AverageRevenuePerUnitReportModel
    {
        [Display(Name = "License Currency Code")]
        public string LicenseCurrencyCode { get; set; }
        public string Duration { get; set; }
        [Display(Name = "Total Revenue")]
        public decimal TotalRevenue { get; set; }
        public int TotalBusinessUnits { get; set; }
    }

    public class BillingRequestsBillingProgressModel
    {
        public string BRReferenceNo { get; set; }

        public string BusinessUnitCode { get; set; }

        [Display(Name = "Business Unit")]
        public string BusinessUnitName { get; set; }

        public string StatusCode { get; set; }

        [Display(Name = "Status")]
        public string StatusName { get; set; }

        public DateTimeOffset? Modified { get; set; }

        [Display(Name = "Approved On")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? ModifiedOn => Modified?.DateTime;

        [Display(Name = "Approved By")]
        public string ModifiedBy { get; set; }
    }

    public class BarChartModel
    {
        //[JsonPropertyName("currency")]
        public string Currency { get; set; }

        //[JsonPropertyName("value")]
        public decimal Value { get; set; }

        //[JsonPropertyName("category")]
        public string Category { get; set; }

        //[JsonPropertyName("color")]
        public string Color { get; set; }

        //[JsonPropertyName("explodeField")]
        public bool ExplodeField { get; set; }

        //[JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }
    }

    public class TreeMapChartModel
    {
        public string Name { get; set; }

        public decimal Value { get; set; }

        public IEnumerable<TreeMapChartModel> Items { get; set; }
    }

    public class LineChartModel
    {
        public string Category { get; set; }
        public decimal Value { get; set; }
        public DateTime Duration { get; set; }
    }

    public class BillingProgressReportModel
    {
        public string Category { get; set; }
        public string PreviousMonth { get; set; }
        public int TotalCount { get; set; }
        public double Value { get; set; }
    }

    public class YearlySalesReportModel
    {
        [Display(Name = "Business Unit")]
        public string BusinessUnitCode { get; set; }

        [Display(Name = "Subscription Type")]
        public string LicenseSubscriptionTypeCode { get; set; }
        public string LicenseCurrencyCode { get; set; }
        public decimal? January { get; set; }
        public decimal? February { get; set; }
        public decimal? March { get; set; }
        public decimal? April { get; set; }
        public decimal? May { get; set; }
        public decimal? June { get; set; }
        public decimal? July { get; set; }
        public decimal? August { get; set; }
        public decimal? September { get; set; }
        public decimal? October { get; set; }
        public decimal? November { get; set; }
        public decimal? December { get; set; }

        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; }

    }
}
