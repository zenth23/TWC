using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.HelperModels
{
    [Serializable]
    public class ReturnMessageModel
    {
        public StatusType Status { get; set; }
        public string Message { get; set; }
        public bool HasError { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
}
}
