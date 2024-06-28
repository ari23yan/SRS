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
    [Table("Reservations", Schema = "General")]

    public class Reservation : BaseEntity
    {
        [Required]
        public Guid TimingId { get; set; }
        [MaxLength(50)]
        public string PatientName { get; set; }
        [MaxLength(60)]
        public string PatientLastName { get; set; }
        [MaxLength(10)]
        public string PatientNationalCode { get; set; }
        [MaxLength(11)]
        public string PatientPhoneNumber { get; set; }
        public string DoctorNonNezam { get; set; }
        public long? RoomCode { get; set; }
        [MaxLength(1028)]
        public string? Description { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool IsConfirmedByMedicalRecords { get; set; } = false; // مدارک پزشکی
        public Guid? ConfirmedMedicalRecordsUserId { get; set; } 
        public bool? IsConfirmedBySupervisor { get; set; } = false;//  سوپروایزر
        public Guid? ConfirmedSupervisorUserId { get; set; }
        public TimeSpan RequestedTime { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
