using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Common
{
    public class DayDto<T>
    {
        public string Day { get; set; }
        public string DayOfTheWeek { get; set; }
        public string Date { get; set; }
        public List<T> Timings { get; set; }
        public List<ReservationDto> Reservations { get; set; }
        public bool IsEnable { get; set; }
        public int? CountPerDay { get; set; }
    }
}
