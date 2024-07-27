using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Timing
{
    public class AddTimingDto
    {
        public string NoNezam { get; set; }
        public long RoomCode { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly[] Date { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public TimeSpan ScheduledDuration
        {
            get
            {
                return  EndTime - StartTime;
            }
        }
    }
}
