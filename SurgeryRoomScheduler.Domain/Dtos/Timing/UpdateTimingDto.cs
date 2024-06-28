using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Timing
{
    public class UpdateTimingDto
    {
        public string? DoctorNoNezam { get; set; }
        public string? DoctorName { get; set; }
        public string? RoomName { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public TimeSpan? ScheduledDuration
        {
            get
            {
                if (ScheduledEndDate.HasValue && ScheduledStartDate.HasValue)
                {
                    return ScheduledEndDate.Value - ScheduledStartDate.Value;
                }
                return null;
            }
        }

        public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
    }
}
