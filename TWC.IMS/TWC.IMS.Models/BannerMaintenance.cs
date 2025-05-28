namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class BannerMaintenance
    {
        public int Id { get; set; }
        public Guid UniqueKey { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; } 
        public string ModifiedBy { get; set; }
        public DateTime? Modified { get; set; }
        public byte[] RowVersion { get; set; }
    }

}
