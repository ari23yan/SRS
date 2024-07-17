using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
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
        Task<IEnumerable<Doctor>> GetDoctorsList(long roomCode,string searchKey);
        Task<IEnumerable<Room>> GetRoomsList(string searchKey);
        Task<IEnumerable<Room>> GetDoctorRooms(string noNezam);
        Task<ListResponseDto<Doctor>> GetDoctorsListPaginated(PaginationDto request);
        Task<bool> DeleteDoctors();
        Task<bool> DeleteDoctorsAssignedRooms();
        Task<bool> DeleteInsurances();
        Task<bool> DeleteRooms();
        Task<bool> DeleteSurgeryNames();
    }
}
