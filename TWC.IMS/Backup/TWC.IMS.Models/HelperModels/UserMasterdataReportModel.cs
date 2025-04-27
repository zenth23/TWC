using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.HelperModels
{
    public class UserMasterdataReportModel
    {
        public string Username { get; set; }
        public string UserRole { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? DeactivationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
