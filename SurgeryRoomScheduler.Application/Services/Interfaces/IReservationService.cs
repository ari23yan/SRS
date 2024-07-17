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
        Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservedList(PaginationDto request,Guid? doctorId, bool isExtera);
        Task<ResponseDto<IEnumerable<ReservationDto>>> GetReservationCancelledList(PaginationDto request,Guid? doctorId);
        Task<ResponseDto<IEnumerable<ReservationDto>>> GetExteraReservationList(PaginationDto request,Guid? doctorId);
        Task<ResponseDto<IEnumerable<TimingDto>>> GetExteraTimingsList(PaginationDto request,Guid? doctorId, long roomCode);

        Task<ResponseDto<IEnumerable<GetTimingDto>>> GetDoctorDayOffList(PaginationDto request, string noNezam, DateOnly startDate, DateOnly endDate);



        Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservervationsList(PaginationDto request,Guid operatorId, ReservationStatus status);
        Task<ResponseDto<IEnumerable<ReservationRejectionAndCancellationReason>>> GetRejectionsReasons(Guid operatorId, bool isCancellation);
        Task<ResponseDto<bool>> ConfirmReservation(Guid reservationId, Guid operatorId);
        Task<ResponseDto<bool>> RejectReservationRequest(RejectReservationRequestDto request, Guid operatorId);
        Task<int> GetReservedCount(string? noNezam);
        Task<int> GetCancelledReservedCount(string? noNezam);
        Task<int> GetExteraReservedCount(long roomCode);
        Task<int> GetReservedConfirmationCountByType(string? operatorType, ReservationStatus status);

        Task<ResponseDto<bool>> CancelReservation(CancelReservationDto request, Guid operatorId);
        Task<ResponseDto<bool>> UpdateReservationByReservationId(Guid reservationId, UpdateReservationDto request, Guid operatorId);

        Task<IEnumerable<ReservationRejectionAndCancellationReason>> GetReservationRejectionReasonByType(RejectionReasonType type);
    }
}
