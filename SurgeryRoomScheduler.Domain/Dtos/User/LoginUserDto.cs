using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.User
{
    public class LoginUserDto
    {
        public string Input { get; set; } // Mobile Or Username
        public string Password { get; set; }
    }
}
