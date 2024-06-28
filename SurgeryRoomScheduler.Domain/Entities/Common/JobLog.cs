using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.Common
{
    [Table("JobLogs", Schema = "Common")]

    public class JobLog
    {
        public Guid Id { get; set; }
        [MaxLength(128)]

        public string JobName { get; set; }           // Name of the job
        [MaxLength(128)]
        public string? Description { get; set; }          
        public bool IsSuccessful { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;    
        public DateTime StartTime { get; set; }       
        public DateTime? EndTime { get; set; }   
        [MaxLength(1024)]

        public string? ErrorDetails { get; set; }      // Error details if the job failed
    }
}
