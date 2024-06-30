using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
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
        Task<IEnumerable<ReservationDto>> GetPaginatedReservedList(PaginationDto request,string noNezam);
        Task<IEnumerable<ReservationDto>> GetPaginatedReservervationsList(PaginationDto request,string operatorType, ReservationStatus status);
        Task<Reservation?> GetReservationById(Guid id);
        Task<ReservationConfirmation?> GetReservationConfirmationById(Guid id);
        Task<ReservationConfirmation?> GetReservationConfirmationByReservationId(Guid id);
        Task<IEnumerable<ReservationRejectionReason>> GetReservationRejectionReasonByType(RejectionReasonType type);

       
    }
}
