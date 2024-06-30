using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ResponseDto<bool>> CreateReservation(AddReservationDto request, Guid operatorId);

        Task<ResponseDto<GetTimingCalenderDto>> GetReservationCalender(GetListByMonthDto request);
        Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservedList(PaginationDto request,Guid? doctorId);
        Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservervationsList(PaginationDto request,Guid operatorId, ReservationStatus status);
        Task<ResponseDto<IEnumerable<ReservationRejectionReason>>> GetRejectionsReasons(Guid operatorId);
        Task<ResponseDto<bool>> ConfirmReservation(Guid reservationId, Guid operatorId);
        Task<ResponseDto<bool>> RejectReservationRequest(RejectReservationRequestDto request, Guid operatorId);
        Task<int> GetReservedCount(string? noNezam);
        Task<int> GetReservedConfirmationCountByType(string? operatorType, ReservationStatus status);

        Task<ResponseDto<bool>> CancelReservation(GetByIdDto request, Guid operatorId);
        Task<ResponseDto<bool>> UpdateReservationByReservationId(Guid reservationId, UpdateReservationDto request, Guid operatorId);

        Task<IEnumerable<ReservationRejectionReason>> GetReservationRejectionReasonByType(RejectionReasonType type);
    }
}
