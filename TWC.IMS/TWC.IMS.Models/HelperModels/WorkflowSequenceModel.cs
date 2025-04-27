using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.HelperModels
{
    [Serializable]
    public class WorkflowSequenceModel
    {
        public bool IsFinal { get; set; }
        public int SortIndex { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmailAddress { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string ApproverEmailAddress { get; set; }
        public int? RejectionReturnIndex { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public int? RejectStatusId { get; set; }
        public string RejectStatus { get; set; }
        public int TimesheetHeaderId { get; set; }
        public DateTime TimesheetPeriodStart { get; set; }
        public DateTime TimesheetPeriodEnd { get; set; }
    }
}
