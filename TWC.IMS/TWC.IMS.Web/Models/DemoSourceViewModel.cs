using Kendo.Mvc.Extensions;
using TWC.IMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TWC.IMS.Web.Models
{
    public class DemoSourceViewModel
    {
        public DemoSourceViewModel()
        {
            DemoSourceDetails = new List<DemoSourceDetailsViewModel>();
        }

        public int? Id { get; set; }

        public Guid? UniqueKey { get; set; }

        public string BRReferenceNo { get; set; }

        public string ToName { get; set; }

        public string FromName { get; set; }

        public DateTime? Date { get; set; }

        public string BusinessUnitCode { get; set; }

        public string BusinessUnitName { get; set; }

        public DateTime? DurationFrom { get; set; }

        public DateTime? DurationTo { get; set; }

        public string BillingAddressedTo { get; set; }

        public string Thru { get; set; }

        public string Note { get; set; }

        public Guid? UploadBatchNumber { get; set; }

        public string WbsLicenseCode { get; set; }

        public string WbsMaintenanceCode { get; set; }

        public string WbsVatableCode { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? LockDatetime { get; set; }

        [Display(Name = "Status")]
        public string CurrentStatus { get; set; }

        public string Remarks { get; set; } //map reject or void

        public bool IsCurrentApprover { get; set; }

        public bool IsCreator { get; set; }

        public int? BillingRequestHeader_WorkflowAction { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? Created { get; set; }

        [StringLength(255)]
        public string ModifiedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? Modified { get; set; }

        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public IList<DemoSourceDetailsViewModel> DemoSourceDetails { get; set; }

    }
}