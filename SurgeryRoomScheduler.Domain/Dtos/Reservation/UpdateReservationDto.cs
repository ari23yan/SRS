using SurgeryRoomScheduler.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Reservation
{
    public class UpdateReservationDto
    {
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
        public TimeSpan RequestedTime { get; set; }
        [SwaggerIgnore]
        public DateTime ModifiedDate { get; set; }
    }
}
