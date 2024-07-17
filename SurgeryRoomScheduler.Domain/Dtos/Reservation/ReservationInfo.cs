using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Reservation
{
    public class ReservationInfo
    {
        public TimeSpan RequestedTime { get; set; }
        public Guid TimingId { get; set; }
    }
}
