using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Timing
{
    public class TimingDto
    {
        public Guid Id { get; set; }
        public string DoctorNoNezam { get; set; }
        public string DoctorName { get; set; }
        public string RoomName { get; set; }
        public long? RoomCode { get; set; }
        public DateOnly? ScheduledDate { get; set; }
        public TimeOnly? ScheduledStartTime { get; set; }
        public TimeOnly? ScheduledEndTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public TimeSpan ScheduledDuration { get; set; }
        public string ScheduledDate_Shamsi { get; set; }
        public string CreatedDate_Shamsi { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

    }
}
