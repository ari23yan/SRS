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

namespace SurgeryRoomScheduler.Domain.Interfaces
{
    public interface IReservationRepository: IRepository<Reservation>
    {
        Task<bool> CheckReservationExist(AddReservationDto request);
        Task<ListResponseDto<ReservationDto>> GetPaginatedReservedList(PaginationDto request, string noNezam, bool isExtera = false);
        Task<ListResponseDto<ReservationDto>> GetReservationCancelledList(PaginationDto request,string noNezam);
        Task<ListResponseDto<ReservationDto>> GetPaginatedReservervationsList(PaginationDto request, string operatorType, ReservationStatus status);
        Task<ListResponseDto<ReservationDto>> GetExteraReservationList(PaginationDto request);
        //Task<ResponseDto<IEnumerable<TimingDto>>> GetExteraTimingsList(PaginationDto request, string roomCode, Guid? doctorId, bool isExtera);
        Task<Reservation?> GetReservationById(Guid id);
        Task<IEnumerable<ReservationDto>> GetDoctorReservationByRoomIdAndDate(long roomCode, string noNezam, DateTime sDate, DateTime eDate);

        Task<ReservationConfirmation?> GetReservationConfirmationById(Guid id);
        Task<ReservationConfirmation?> GetReservationConfirmationByReservationId(Guid id);
        Task<IEnumerable<ReservationRejectionAndCancellationReason>> GetReservationRejectionReasonByType(RejectionReasonType type);

       
    }
}
