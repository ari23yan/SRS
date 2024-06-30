using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Enums
{
    public enum ReservationConfirmationStatus
    {
        Pending,

        ApprovedBySupervisor,

        ApprovedByMedicalRecord,

        RejectedByMedicalRecord,

        RejectedBySupervisor,
    }

}
