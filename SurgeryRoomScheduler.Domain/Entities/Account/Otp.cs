using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Account
{
    [Table("Otps", Schema = "Account")]

    public class Otp
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        [MaxLength(6)]
        public string OtpCode { get; set; }
        [Required]
        public DateTime ExpirationTime { get; set; } = DateTime.Now.AddMinutes(2);
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
