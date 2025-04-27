using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models
{
    [Serializable]
    [Table("SystemNotifications")]
    public class SystemNotification : DescribableEntity
    {
        [Key]
        public int Id { get; set; }
        public Guid UniqueKey { get; set; }
        [Required]
        public string Title { get; set; }
        [System.Web.Mvc.AllowHtml]
        public string Caption { get; set; }
        [System.Web.Mvc.AllowHtml]
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime Created { get; set; }
        public DateTime? SeenDate { get; set; }
        public bool IsViewed { get; set; }

        public int SystemNotification_UserDetail { get; set; }
        public virtual UserDetail UserDetail { get; set; }

    }
}
