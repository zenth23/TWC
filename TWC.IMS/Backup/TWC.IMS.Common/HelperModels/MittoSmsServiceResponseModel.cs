using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperModels
{
    public class MittoSmsServiceResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("sid")]
        public string Sid { get; set; }

        [JsonProperty("valid_until")]
        public DateTime? ValidUntil { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }
    }
}
