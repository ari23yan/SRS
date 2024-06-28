using SurgeryRoomScheduler.Domain.Entities.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Common
{
    [Table("Menus", Schema = "Common")]

    public class Menu : BaseEntity
    {
        public Guid Id { get; set; }
        [MaxLength(256)]
        public string? Name { get; set; }
        [MaxLength(256)]
        public string? Name_Farsi { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? Link { get; set; }
        public ICollection<Role> RolePermissions { get; set; }
        public Guid? SubMenuId { get; set; }
    }
}
