using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class DemoSourceDetailsViewModel
    {
        public int? Id { get; set; }

        public Guid? UniqueKey { get; set; }

        public string WbsCode { get; set; }

        public DateTime? Duration { get; set; }

        public string Particular { get; set; }

        public int? Qty { get; set; }

        public decimal? UnitPrice { get; set; }

        public bool IncludeInSum { get; set; }

        public decimal? TotalAmount { get; set; }

        public string LicenseCurrencyCode { get; set; }

        public string WbsSubscriptionTypeCode { get; set; }

        public string LicenseSubscriptionTypeCode { get; set; }

        public int? BillingRequestDetail_BillingRequestHeader { get; set; }

        public string LicenseCode { get; set; }

        public string Product { get; set; }

    }
}