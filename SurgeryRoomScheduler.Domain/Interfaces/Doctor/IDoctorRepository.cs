using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Interfaces
{
    public interface IDoctorRepository: IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetDoctorsList(string searchKey);
        Task<IEnumerable<Room>> GetRoomsList(string searchKey);
        Task<IEnumerable<Room>> GetDoctorRooms(string noNezam);
        Task<bool> DeleteDoctors();
    }
}
