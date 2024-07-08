using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("Insurances", Schema = "General")]

    public class Insurance
    {
        public long Id { get; set; }
        public long Code { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
    }
}
