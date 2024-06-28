using SurgeryRoomScheduler.Domain.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Permission
{
    public class PermissionGroupDto
    {
        public Guid Id { get; set; }
        public string PermissionGroupName { get; set; }
        public string PermissionGroupName_Farsi { get; set; }
        public string? Description { get; set; }
        public List<PermissionsDto> Permissions { get; set; }

    }
}
