using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Insurance;
using SurgeryRoomScheduler.Domain.Dtos.SurgeryName;
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
        Task<IEnumerable<Insurance>> GetInsuranceList(string searchKey);
        Task<IEnumerable<SurgeryName>> GetSurgeryNamesList(string searchKey);
        Task<ListResponseDto<Doctor>> GetDoctorsListPaginated(PaginationDto request);
        Task<bool> DeleteDoctors();
        Task<bool> DeleteDoctorsAssignedRooms();
        Task<bool> DeleteInsurances();
        Task<bool> DeleteRooms();
        Task<bool> DeleteSurgeryNames();
    }
}
