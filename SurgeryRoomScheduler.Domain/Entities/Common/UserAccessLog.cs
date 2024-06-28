using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Common
{
    [Table("UsersAccessLog", Schema = "Common")]
    public class UsersAccessLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OperatorId { get; set; }
        public AccessLogType Type { get; set; }
        [MaxLength(256)]
        public string? Action { get; set; }
        public DateTime InsertDate { get; set; } = DateTime.Now;
    }
}
