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
    public class DatabaseArchivingLogsViewModel: DatabaseArchivingLog
    {
        [Display(Name = "Timestamp")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime? CreatedOn
        {
            get
            {
                return base.Created == null ? null : base.Created.Value.DateTime.AsNullable();
            }
        }
    }
}