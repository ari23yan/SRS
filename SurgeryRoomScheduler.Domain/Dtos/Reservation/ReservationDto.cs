using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Reservation
{
    public class ReservationDto
    {
        public Guid? Id { get; set; }
        public Guid TimingId { get; set; }
        public string PatientName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientNationalCode { get; set; }
        public string PatientPhoneNumber { get; set; }
        public bool PatientHaveInsurance { get; set; }
        public string DoctorNoNezam { get; set; }
        public string DoctorName { get; set; }
        public string RoomName { get; set; }
        public long? RoomCode { get; set; }
        public string? Description { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool IsConfirmedByMedicalRecords { get; set; } = false; // مدارک پزشکی
        public Guid? ConfirmedMedicalRecordsUserId { get; set; }
        public bool IsConfirmedBySupervisor { get; set; } = false;//  سوپروایزر
        public Guid? ConfirmedSupervisorUserId { get; set; }
        public TimeSpan RequestedTime { get; set; }
        public int StatusType { get; set; }
        public string Status { get; set; }
        public bool IsExtera { get; set; }

        public Guid? ReservationCancelationReasonId { get; set; }
        public string? ReservationCancelationReason { get; set; }

        public string? CancelationDescription { get; set; }

    }
}
