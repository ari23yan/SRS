using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Reservation
{
    public class CancelReservationDto
    {
        public Guid ReservationId { get; set; }
        public string? CancellationDescription { get; set; }
        public Guid ReservationCancellationReasonId { get; set; }
    }
}
