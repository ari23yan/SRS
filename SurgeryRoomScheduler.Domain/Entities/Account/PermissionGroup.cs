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
    [Table("PermissionGroup", Schema = "Account")]

    public class PermissionGroup: BaseEntity
    {
        [MaxLength(128)]
        public string PermissionGroupName { get; set; }
        [MaxLength(128)]
        public string PermissionGroupName_Farsi { get; set; }
        [MaxLength(1024)]
        public string? Description { get; set; }
        public ICollection<PermissionGroup_Permission> permissionGroup_Permissions { get; set; }
    }
}
