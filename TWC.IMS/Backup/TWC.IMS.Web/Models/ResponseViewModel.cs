using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class ResponseViewModel : TWC.IMS.Common.HelperClasses.DescribableEntity
    {
        public int WorkflowActionId { get; set; }
        public int WorkflowApprovalId { get; set; }
        public string Remarks { get; set; }

        public int RequestId { get; set; }

        public int? ReturnTo { get; set; }
        public int? ReturnWfaSortIndex { get; set; }
    }
}