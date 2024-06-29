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
    [Table("ReservationConfirmations", Schema = "General")]

    public class ReservationConfirmation:BaseEntity
    {
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public Guid OperationTypeId { get; set; }
        public ReservationConfirmationType OperationType { get; set; }
        public Guid StatusId { get; set; }
        public ReservationConfirmationStatus Status { get; set; }
        public bool IsConfirmedByMedicalRecords { get; set; } = false; // مدارک پزشکی
        public Guid? ConfirmedMedicalRecordsUserId { get; set; }
        public bool? IsConfirmedBySupervisor { get; set; } = false;//  سوپروایزر
        public Guid? ConfirmedSupervisorUserId { get; set; }
        public Guid? ReservationRejectionId { get; set; }
        public ReservationRejection ReservationRejection { get; set; }
    }
}
