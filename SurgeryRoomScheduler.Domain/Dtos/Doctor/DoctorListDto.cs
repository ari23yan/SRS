using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos
{
    public class DoctorListDto
    {
        public long Id { get; set; }
        public string? FullName { get; set; }
        public string? NoNezam { get; set; }
    }
}
