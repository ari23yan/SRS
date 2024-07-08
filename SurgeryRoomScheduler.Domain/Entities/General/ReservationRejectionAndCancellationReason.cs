using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Entities.General
{
    [Table("ReservationRejectionAndCancellationReasons", Schema = "General")]
    public class ReservationRejectionAndCancellationReason: BaseEntity
    {
        public RejectionReasonType RejectionReasonType { get; set; }
        [MaxLength(5000)]
        public string Reason { get; set; }
    }
}
