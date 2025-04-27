namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReportCaches")]
    public partial class ReportCache
    {
        [Key]
        public int Id { get; set; }
        
        public Guid UniqueKey { get; set; }
        
        [StringLength(255)]
        public string ReportName { get; set; }
        
        public string ReportImage { get; set; }

        public DateTime ExpirationDate { get; set; }
        
        [StringLength(255)]
        public string ContentType { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTimeOffset? Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}