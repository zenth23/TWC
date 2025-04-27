using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWC.IMS.Models
{
    [Serializable]
    [Table("SignalRConnections")]
    public partial class SignalRConnection : DescribableEntity
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public DateTime Created { get; set; }
    }
}
