using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.General;

namespace SurgeryRoomScheduler.Domain.Dtos
{
    public class GetExteraTimingDto
    {
        public IEnumerable<SurgeryRoomScheduler.Domain.Entities.General.Timing?> UnreservedTimings { get; set; }
        public IEnumerable<NotFullyReservedTimingsDto?> NotFullyReservedTimings { get; set; }
    }

}
