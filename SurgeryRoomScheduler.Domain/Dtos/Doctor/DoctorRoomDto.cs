using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Doctor
{
    public class DoctorRoomDto
    {
        public string NoNezam { get; set; }
        public long RoomId { get; set; }
    }
}
