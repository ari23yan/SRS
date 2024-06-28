using SurgeryRoomScheduler.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("OperationConfirmations", Schema = "General")]

    public class OperationConfirmation:BaseEntity
    {
        public Guid OperationTypeId { get; set; }
        public OperationConfirmationType OperationType { get; set; }
        public Guid StatusId { get; set; }
        public OperationConfirmationStatus Status { get; set; }
        [MaxLength(200)]
        public string? ConfirmationDetails { get; set; }
    }
}
