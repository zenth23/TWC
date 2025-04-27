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
    public class AccessViewModel : Access
    {
        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:" + "TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1" + "}")]
        public DateTime? CreatedOn
        {
            get
            {
                return base.Created == null ? null : base.Created.Value.DateTime.AsNullable();
            }
        }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:" + "TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1" + "}")]
        public DateTime? ModifiedOn
        {
            get
            {
                return base.Modified == null ? null : base.Modified.Value.DateTime.AsNullable();
            }
        }

        [Timestamp]
        public byte[] AccessRowVersion { get; set; }

        public bool IsChecked { get; set; }
    }
}