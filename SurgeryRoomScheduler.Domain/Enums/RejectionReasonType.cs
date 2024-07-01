using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Enums
{
    public enum RejectionReasonType:byte
    {
        Supervisor = 1,
        MedicalRecords,
        doctor
    }
}
