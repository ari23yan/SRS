using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Role
{
    public class MenusDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Name_Farsi { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public Guid? SubMenuId { get; set; }
    }
}
