using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Common
{
    [Table("OperationConfirmationLogs", Schema = "Common")]
    public class OperationConfirmationLog:BaseEntity
    {
        public Guid OperationConfirmationId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid ConfirmedByUserId { get; set; }
        [Required]
        public Guid StatusId { get; set; }
        public OperationConfirmationStatus Status { get; set; }
        [MaxLength(250)]
        public string? LogDetails { get; set; }
        public DateTime LogDate { get; set; }
    }
}
