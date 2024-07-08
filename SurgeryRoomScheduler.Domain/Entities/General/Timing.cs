using SurgeryRoomScheduler.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("Timings", Schema = "General")]

    public class Timing:BaseEntity
    {
        public string? AssignedDoctorNoNezam { get; set; }
        public long AssignedRoomCode { get; set; }
        public TimeOnly ScheduledStartTime { get; set; }
        public TimeOnly ScheduledEndTime { get; set; }
        public DateOnly ScheduledDate { get; set; }
        public TimeSpan ScheduledDuration { get; set; }
        public string ScheduledDate_Shamsi { get; set; }
        public string CreatedDate_Shamsi { get; set; }
        public bool IsExtraTiming { get; set; } = false; // مازاد
        public string? PreviousOwner { get; set; } //Doctor NoNezam 
    }
}
