using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Common
{
    public class GetPersianDateDto
    {
        public string? PersianDate { get; set; }
        public string? PersianMonth { get; set; }
        public string? PersianDayOfWeek { get; set; }
    }
}
