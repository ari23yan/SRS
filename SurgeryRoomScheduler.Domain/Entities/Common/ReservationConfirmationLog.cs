using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Common
{
    [Table("ReservationConfirmationLogs", Schema = "Common")]
    public class ReservationConfirmationLog:BaseEntity
    {
        public Guid OperatorId { get; set; }
        public ConfirmationAction ConfirmationAction { get; set; }
        [MaxLength(250)]
        public string? LogDetails { get; set; }
        public Guid ReservationConfirmationTypeId { get; set; }
        public ReservationConfirmationType ReservationConfirmationType { get; set; }
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
