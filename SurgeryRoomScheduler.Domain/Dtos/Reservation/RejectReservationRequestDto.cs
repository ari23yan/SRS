using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Reservation
{
    public class RejectReservationRequestDto
    {
        public Guid ReservationId { get; set; }
        public string? AdditionalDescription { get; set; }
        public Guid ReservationRejectionReasonId { get; set; }
    }
}
