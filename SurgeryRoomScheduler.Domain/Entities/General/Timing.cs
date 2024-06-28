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
        public string AssignedDoctorNoNezam { get; set; }
        public long? AssignedRoomCode { get; set; }
        public DateTime ScheduledStartDate { get; set; }
        public DateTime ScheduledEndDate { get; set; }
        public TimeSpan ScheduledDuration { get; set; }
        public string ScheduledStartDate_Shamsi { get; set; }
        public string ScheduledEndDate_Shamsi { get; set; }
        public string CreatedDate_Shamsi { get; set; }
    }
}
