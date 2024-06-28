using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("DoctorRooms", Schema = "General")]
    public class DoctorRoom
    {
        public long Id { get; set; }
        [MaxLength(128)]
        public string NoNezam { get; set; }
        public long RoomCode { get; set; }
    }
}
