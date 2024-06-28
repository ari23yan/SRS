using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Permission
{
    public class PermissionsDto
    {
        public Guid Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionName_Farsi { get; set; }
        public string? Description { get; set; }
        public bool HasPermission { get; set; } 

    }
}
