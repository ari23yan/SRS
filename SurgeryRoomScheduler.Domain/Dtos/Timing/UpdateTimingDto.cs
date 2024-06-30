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


        public string NoNezam { get; set; }
        public long RoomCode { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly Date { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public TimeSpan ScheduledDuration
        {
            get
            {
                return EndTime - StartTime;
            }
        }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
    }
}
