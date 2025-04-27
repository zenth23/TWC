using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.HelperModels
{
    public class AccessControlListReportModel
    {
        public int No { get; set; }
        public string Username { get; set; }
        public string EmployeeId { get; set; }
        public string FullName
        {
            get
            {
                var middle = !string.IsNullOrWhiteSpace(this.MiddleName) ? " " + this.MiddleName : string.Empty;
                return $"{this.LastName}, {this.FirstName} {middle}";
            }
        }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
        public string Modules { get; set; }
        public DateTime? ActivationDatetime { get; set; }
        public DateTime? DeactivationDatetime { get; set; }
        public DateTime? LastLoginDatetime { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? DaysInactive { get; set; }

        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string FirstName { get; set; }
        [NotMapped]
        public string MiddleName { get; set; }
        [NotMapped]
        public string LastName { get; set; }
    }
}
