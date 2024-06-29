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
        public DateTime? ScheduledDate { get; set; }
        public TimeSpan? ScheduledTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime ScheduledStartDate { get; set; }
        public DateTime ScheduledEndDate { get; set; }
        public TimeSpan ScheduledDuration { get; set; }
        public string ScheduledStartDate_Shamsi { get; set; }
        public string ScheduledEndDate_Shamsi { get; set; }
        public string CreatedDate_Shamsi { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

    }
}
