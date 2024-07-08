using SurgeryRoomScheduler.Domain.Dtos.Permission;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.User
{
    public class UserDetailDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Username { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public Guid? IsCreatedBy { get; set; }
        public Guid RoleId { get; set; }
        //public DateTime? DateOfBirth { get; set; }
        //public DateTime? LastLoginDate { get; set; }
        //public string? ProfilePicture { get; set; }
        public string RoleName { get; set; }
        public string RoleName_Farsi { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsModified { get; set; } = false;
        public GenderType Gender { get; set; } 
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        //public PermissionGroupDto PermissionGroup { get; set; }
        public List<PermissionGroupDto> PermissionGroup { get; set; }

    }
}
