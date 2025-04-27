using Kendo.Mvc.Extensions;
using TWC.IMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    [MetadataType(typeof(Request))]
    public class RequestViewModel : Request
    {
        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime CreatedOn
        {
            get
            {
                return  base.Created.DateTime;
            }
        }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? ModifiedOn
        {
            get
            {
                return base.Modified == null ? null : base.Modified.Value.DateTime.AsNullable();
            }
        }

        public byte[] ConfigRowVersion { get; set; }

        [Display(Name = "Status")]
        public string StrStatus { get; set; }
        public int? ApprovalEntityId { get; set; }

        public bool UserHasPendingApproval { get; set; }
        public int? UserPendingApprovalId { get; set; }
        public string RequestName { get; set; }
    }
}