using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos
{
    public class GetReservationCalenderDto
    {
        public string MonthName { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public List<DayDto<ReservationDto>> Days { get; set; }
    }
}
