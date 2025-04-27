using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWC.IMS.Models
{
    [Serializable]
    [Table("StatusSets")]
    public partial class StatusSet : DescribableEntity
    {
        public StatusSet()
        {
            // REQUEST FOR REMOVAL
            Requests = new HashSet<Request>();
  
        }

        [Key]
        public int Id { get; set; }
        public Guid UniqueKey { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Module { get; set; }

        [Display(Name = "Created By")]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Created { get; set; }

        [Display(Name = "Modified By")]
        [StringLength(255)]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified")]
        [DisplayFormat(DataFormatString = "{0:" + TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1 + "}")]
        public DateTimeOffset? Modified { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        // REQUEST FOR REMOVAL
        public virtual ICollection<Request> Requests { get; set; }
   
    }
}
