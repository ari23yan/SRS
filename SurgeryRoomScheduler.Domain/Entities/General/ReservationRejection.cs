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
    [Table("ReservationRejections", Schema = "General")]
    public class ReservationRejection:BaseEntity
    {
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public Guid ReservationRejectionReasonId { get; set; }
        public ReservationRejectionReason ReservationRejectionReason { get; set; }
        [MaxLength(5000)]
        public string? AdditionalDescription { get; set; }
    }
}
