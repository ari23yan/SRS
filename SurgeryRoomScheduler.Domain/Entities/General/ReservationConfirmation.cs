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
    [Table("ReservationConfirmations", Schema = "General")]

    public class ReservationConfirmation:BaseEntity
    {
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public Guid ReservationConfirmationTypeId { get; set; }
        public ReservationConfirmationType ReservationConfirmationType { get; set; }
        public int StatusId { get; set; } = 1;
        public ReservationConfirmationStatus Status { get; set; } 
        public bool IsConfirmedByMedicalRecords { get; set; } = false; // مدارک پزشکی
        public Guid? ConfirmedMedicalRecordsUserId { get; set; }
        public bool IsConfirmedBySupervisor { get; set; } = false;//  سوپروایزر
        public Guid? ConfirmedSupervisorUserId { get; set; }

        public Guid? ReservationRejectionAndCancellationReasonId { get; set; }
        public ReservationRejectionAndCancellationReason ReservationRejectionAndCancellationReason { get; set; }

        [MaxLength(5000)]
        public string? RejectionAndCancellationAdditionalDescription { get; set; }

        //public Guid? ReservationRejectionId { get; set; }
        //public ReservationRejection ReservationRejection { get; set; }
        public Guid? RejectionAndCancellationUserId { get; set; }
    }
}
