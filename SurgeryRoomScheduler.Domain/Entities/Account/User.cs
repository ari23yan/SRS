using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Account
{
    [Table("Users", Schema = "Account")]

    public class User : BaseEntity
    {
        [MaxLength(64)]
        public string? NoNezam { get; set; }
        [MaxLength(64)]
        public string? FirstName { get; set; }
        [MaxLength(64)]
        public string? LastName { get; set; }
        [MaxLength(64)]
        public string Username { get; set; }
        [MaxLength(20)]
        public string NationalCode { get; set; }
        [MaxLength(128)]
        public string Password { get; set; }
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        [MaxLength(64)]
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public DateTime? LastLoginDate { get; set; }
        //public UserDetail UserDetail { get; set; }
        public ICollection<Otp> Otps { get; set; } = new List<Otp>();
    }
}
