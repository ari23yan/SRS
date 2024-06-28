using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("Rooms", Schema = "General")]

    public class Room
    {
        public long Id { get; set; }
        [MaxLength(256)]
        public string? Name { get; set; }
        public long? Code { get; set; }
        public bool? IsActive { get; set; }
    }
}
