using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Common
{
    [Table("ApplicationLog", Schema = "Common")]

    public class ApplicationLog
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Source { get; set; }
        public string? InnerException { get; set; }
    }
}
