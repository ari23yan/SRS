using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Common
{
    public class DayOfDateDto
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public WeekDays DayOfWeek { get; set; }
    }
    public class GetDayOfDateDto
    {
        public string DateShamsi { get; set; }
        public string DateMiladi { get; set; }
        public string DayOfWeek { get; set; }
    }


}
