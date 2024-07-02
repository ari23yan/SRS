using SurgeryRoomScheduler.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("ReservationConfirmationStatuses", Schema = "General")]

    public class ReservationConfirmationStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [MaxLength(200)]

        public string? Description { get; set; }
    }
}
