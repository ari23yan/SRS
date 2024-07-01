using SurgeryRoomScheduler.Domain.Dtos.Permission;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.User
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Username { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        //public GenderType? Gender { get; set; }
        //public DateTime? DateOfBirth { get; set; }
        //public string? ProfilePicture { get; set; }
        public bool? IsActive { get; set; }
        public Guid? RoleId { get; set; }
    }
}
