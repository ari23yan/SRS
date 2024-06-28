using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos
{
    public class RoomsListDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long? Code { get; set; }
    }
}
