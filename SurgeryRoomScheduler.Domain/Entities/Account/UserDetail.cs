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
    [Table("UserDetails", Schema = "Account")]
    public class UserDetail: BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? LastLoginDate { get; set; }
        [MaxLength(256)]
        public string? ProfilePicture { get; set; }
    }
}
