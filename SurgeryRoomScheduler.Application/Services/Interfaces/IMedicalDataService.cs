using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Interfaces
{
    public interface IMedicalDataService
    {
        Task<ResponseDto<IEnumerable<DoctorListDto>>> GetDoctorList(long roomCode,string searchKey);
        Task<ResponseDto<IEnumerable<DoctorListDto>>> GetDoctorListPaginated(PaginationDto request);
        Task<ResponseDto<IEnumerable<RoomsListDto>>> GetRoomsList(string searchKey);
        Task<ResponseDto<IEnumerable<RoomsListDto>>> GetDoctorRooms(string noNezam);
        Task<bool> DeleteDoctors();
        Task<bool> DeleteDoctorsAssignedRooms();
        Task<bool> DeleteInsurances();
        Task<bool> DeleteRooms();
        Task<bool> DeleteSurgeryNames();
    }
}
