using SurgeryRoomScheduler.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Account
{
    [Table("Permissions", Schema = "Account")]

    public class Permission:BaseEntity
    {
        [MaxLength(128)]
        public string PermissionName { get; set; }
        [MaxLength(128)]
        public string PermissionName_Farsi { get; set; }
        [MaxLength(1024)]
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
        public ICollection<PermissionGroup_Permission> PermissionGroup_Permissions { get; set; }
    }
}
