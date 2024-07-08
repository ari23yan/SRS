using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Timing
{
    public class NotFullyReservedTimingsDto
    {
        public Guid TimingId { get; set; }
        public string? AssignedDoctorNoNezam { get; set; }
        public long AssignedRoomCode { get; set; }
        public TimeOnly ScheduledStartTime { get; set; }
        public TimeOnly ScheduledEndTime { get; set; }
        public DateOnly ScheduledDate { get; set; }
        public TimeSpan ScheduledDuration { get; set; }
        public string ScheduledDate_Shamsi { get; set; }
        public string CreatedDate_Shamsi { get; set; }
        public bool IsExtraTiming { get; set; } = false;
        public Guid? PreviousOwner { get; set; }
        public TimeSpan UsageTime { get; set; } // مدت زمان استفاده از زمان بندی 
    }
}
