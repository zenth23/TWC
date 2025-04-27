namespace TWC.IMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class order_details
    {
        [Key]
        public int order_detail_id { get; set; }

        public int? order_id { get; set; }

        public int? inventory_id { get; set; }

        public int quantity { get; set; }

        public decimal price { get; set; }

 
    }
}
