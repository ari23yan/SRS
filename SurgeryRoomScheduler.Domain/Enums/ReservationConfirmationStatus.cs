using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Enums
{
    public enum ReservationConfirmationStatus
    {
        Pending=1,
        ApprovedBySupervisor,
        ApprovedByMedicalRecord,
        RejectedByMedicalRecord,
        RejectedBySupervisor,
        CancelledByDoctor,
    }

}
