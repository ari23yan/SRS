using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.SurgeryName
{
    public class SurgeryNameDto
    {
        public int code { get; set; }
        public string name_amal { get; set; }
        public bool Active { get; set; }
    }
}
