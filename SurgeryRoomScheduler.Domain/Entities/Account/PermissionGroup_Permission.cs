using SurgeryRoomScheduler.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Account
{
    [Table("PermissionGroup_Permissions", Schema = "Account")]

    public class PermissionGroup_Permission: BaseEntity
    {
        public Guid PermissionGroupId { get; set; }
        public Guid PermissionId { get; set; }
        public PermissionGroup PermissionGroup { get; set; }
        public Permission Permission { get; set; }
    }
}
