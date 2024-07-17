using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Timing
{
    public class SubmitDoctorDayOffDto
    {
        public Guid[] TimingId { get; set; }
    }
}
