using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Web.Models
{
    public class SystemNotificationViewModel : TWC.IMS.Models.SystemNotification
    {
        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTime CreatedOn
        {
            get
            {
                return base.Created;
            }
        }

        public string StrCreatedOn
        {
            get
            {
                return base.Created.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1);
            }
        }

    }
}
