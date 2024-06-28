using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Enums
{
    public enum  UserAuthResponse 
    {
        Success,
        NotFound,
        WrongPassword,
        NotAvtive,
        IsDeleted,
        TooManyTries,
    }
}
