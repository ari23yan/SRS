using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Interfaces
{
    public interface ITimingRepository:IRepository<Timing>
    {
        Task<IEnumerable<TimingDto>> GetPaginatedTimingList(PaginationDto request);
        Task<IEnumerable<TimingDto>> GetPaginatedTimingListByRoomAndDate(PaginationDto request, long roomCode, DateOnly date);
        Task<IEnumerable<TimingDto>> GetTimingByRoomIdAndDate(long roomCode,DateTime sDate,DateTime eDate);
        Task<IEnumerable<TimingDto>> GetDoctorTimingByRoomIdAndDate(long roomCode,string noNezam,DateTime sDate,DateTime eDate);
        Task<TimingDto> GetTimingDetailByTimingId(Guid timingId);
        Task<bool> CheckTimingExist(AddTimingDto request);
        Task<Timing?> GetTimingById(Guid timingId);
        Task<IEnumerable<Timing>> GetTimingListByDate(DateOnly date);

    }
}
