using SurgeryRoomScheduler.Domain.Entities.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Role
{
    public class UpdateRoleDto
    {
        public string RoleName { get; set; }
        public string RoleName_Farsi { get; set; }
        public string? Description { get; set; }
        public Guid[] RolePermissions { get; set; }
        public Guid[] MenuId { get; set; }
    }
}
