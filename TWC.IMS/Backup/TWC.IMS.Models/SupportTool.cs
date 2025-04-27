namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Serializable]
    [Table("SQLTables")]
    public class SQLTable
    {
        [Key]
        [Display(Name = "Table Id")]
        public int TableId { get; set; }
        [StringLength(255)]
        [Display(Name = "Table Name")]
        public String TableName { get; set; }
    }

    [Serializable]
    [Table("SQLColumns")]
    public class SQLColumn
    {
        [Key]
        [Column(Order = 1)]
        [Display(Name = "Column Id")]
        public int ColumnId { get; set; }
        [Key]
        [Column(Order = 2)]
        [Display(Name = "Table Id")]
        public int TableId { get; set; }
        [StringLength(255)]
        [Display(Name = "Column Name")]
        public String ColumnName { get; set; }
        [StringLength(255)]
        [Display(Name = "Data Type")]
        public String DataType { get; set; }
    }
}
