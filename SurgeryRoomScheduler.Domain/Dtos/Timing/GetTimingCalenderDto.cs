using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Timing
{
    public class GetTimingCalenderDto
    {
        public string MonthName { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public List<DayDto<TimingDto>> Days { get; set; }
        public List<DayDto<SurgeryRoomScheduler.Domain.Entities.General.Reservation>> Reserved { get; set; }
    }
}
