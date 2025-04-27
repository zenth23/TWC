using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWC.IMS.Models;

namespace TWC.IMS.Web.Models
{
    [NotMapped]
    public class SalesOrderViewModel : TWC.IMS.Models.SalesOrderHeader
    {
        public string SupplierName { get; set; }
        public string LocationName { get; set; }
        public string ProductName { get; set; }

        public int quantity { get; set; }
        public decimal Cost { get; set; }

        [Required]
        public int? product_id { get; set; }
        public bool IsGold { get; set; }
        public decimal? Weight { get; set; }

        public string ValidationMessage { get; set; }
    }
}
