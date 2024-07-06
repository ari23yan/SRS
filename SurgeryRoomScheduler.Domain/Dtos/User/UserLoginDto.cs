using SurgeryRoomScheduler.Domain.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.User
{
    public class UserLoginDto
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NoNezam { get; set; }
        public GetRoleMenuDto RoleMenus { get; set; }
    }
}
