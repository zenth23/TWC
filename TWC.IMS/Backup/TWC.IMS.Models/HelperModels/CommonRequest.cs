using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.HelperModels
{
    public class CommonRequest
    {
        public int Id { get; set; }
        public Guid UniqueKey { get; set; }
        public string TransactionNumber { get; set; }
        public string Url { get; set; }
        public int ProponentId { get; set; }
    }
}
