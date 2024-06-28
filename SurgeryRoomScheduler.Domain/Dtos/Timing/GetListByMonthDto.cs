using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Timing
{
    public class GetListByMonthDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public long RoomCode { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public Guid? UserId { get; set; }
    }
}
