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

        public DateTime ScheduledStartDate { get; set; }
        public DateTime ScheduledEndDate { get; set; }
        [SwaggerSchema(ReadOnly = true)]

        public TimeSpan ScheduledDuration
        {
            get
            {
                return ScheduledEndDate - ScheduledStartDate;
            }
        }
    }
}
