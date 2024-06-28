using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Sender
{
    public class ConfirmOtpDto
    {
        public string Otp { get; set; }
        public string PhoneNumber { get; set; }
    }
}
