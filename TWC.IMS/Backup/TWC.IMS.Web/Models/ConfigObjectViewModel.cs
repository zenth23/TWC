using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    public class ConfigObjectViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public int StatusId { get; set; }

        [Required]
        [UIHint("StatusListMultiple")]
        public virtual IEnumerable<StatusViewModel> Status { get; set; }
    }

    [Serializable]
    public class StatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}