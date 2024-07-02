using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Interfaces
{
    public interface ITimingService
    {
        Task<ResponseDto<IEnumerable<TimingDto>>> GetPaginatedTimingList(PaginationDto request);
        Task<ResponseDto<IEnumerable<TimingDto>>> GetPaginatedTimingListByRoomAndDate(PaginationDto request, long roomCode, DateOnly date);
        Task<ResponseDto<IEnumerable<TimingDto>>> GetTimingListByDate(DateOnly date);
        Task<ResponseDto<GetTimingCalenderDto>> GetTimingCalender(GetListByMonthDto request);
        Task<int> GetTimingsCount();
        Task<ResponseDto<TimingDto>> GetTimingDetailByTimingId(Guid timingId);
        Task<ResponseDto<bool>> CreateTiming(AddTimingDto request, Guid operatorId);
        Task<ResponseDto<bool>> DeleteTimingByTimingId(GetByIdDto request, Guid operatorId);
        Task<ResponseDto<bool>> UpdateTimingByTimingId(Guid timingId, UpdateTimingDto request, Guid operatorId);
    }
}
