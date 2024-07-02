using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Reservation
{
    public class AddReservationDto
    {
        public Guid TimingId { get; set; }
        public string PatientName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientNationalCode { get; set; }

        public string PatientPhoneNumber { get; set; }
        public bool PatientHaveInsurance { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string DoctorNoNezam { get; set; }
        [SwaggerSchema(ReadOnly = true)]

        public long RoomCode { get; set; }
        public string? Description { get; set; }
        //public DateTime RequestedDate { get; set; }
        public TimeSpan RequestedTime { get; set; }
    }
}
