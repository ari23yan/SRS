using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Common
{
    public class PersianDayInfoDto
    {
        public string Day { get; set; }
        public string DayOfTheWeek { get; set; }
        public string ShamsiDate { get; set; }
        public DateTime MiladiDate { get; set; }
        public bool IsEnable { get; set; }
    }
}
