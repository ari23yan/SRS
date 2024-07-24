using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationConfirmationStatus = SurgeryRoomScheduler.Domain.Enums.ReservationConfirmationStatus;

namespace SurgeryRoomScheduler.Data.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckReservationExist(AddReservationDto request)
        {
            return await Context.Reservations
                      .AnyAsync(x => x.DoctorNoNezam.Equals(request.DoctorNoNezam) &&
                      x.RoomCode.Equals(request.RoomCode) &&
                      x.TimingId.Equals(request.TimingId));
        }

        public async Task<IEnumerable<ReservationDto>> GetDoctorReservationByRoomIdAndDate(long roomCode, string noNezam, DateTime sDate, DateTime eDate)
        {
            var baseQuery = from reservation in Context.Reservations
                            join doctor in Context.Doctors on reservation.DoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on reservation.RoomCode equals room.Code
                            join reservationConfirmation in Context.ReservationConfirmations on reservation.Id equals reservationConfirmation.ReservationId
                            join reservationConfirmationStatus in Context.ReservationConfirmationStatuses on reservationConfirmation.StatusId equals reservationConfirmationStatus.Id
                            join timing in Context.Timings on reservation.TimingId equals timing.Id
                            where !reservation.IsDeleted && reservation.IsActive && reservationConfirmation.IsActive && !reservationConfirmation.IsDeleted
                            && reservation.RoomCode == roomCode && reservation.DoctorNoNezam.Equals(noNezam)
                            && reservation.RequestedDate.Date >= sDate.Date.Date
                            && reservation.RequestedDate.Date <= eDate.Date.Date
                            select new ReservationDto
                            {
                                Id = reservation.Id,
                                TimingId = reservation.TimingId,
                                PatientName = reservation.PatientName,
                                PatientLastName = reservation.PatientLastName,
                                PatientNationalCode = reservation.PatientNationalCode,
                                PatientPhoneNumber = reservation.PatientPhoneNumber,
                                DoctorNoNezam = doctor.NoNezam,
                                DoctorName = doctor.Name,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                Description = reservation.Description,
                                RequestedDate = reservation.RequestedDate,
                                IsConfirmedByMedicalRecords = reservationConfirmation.IsConfirmedByMedicalRecords,
                                IsConfirmedBySupervisor = reservationConfirmation.IsConfirmedBySupervisor,
                                ConfirmedMedicalRecordsUserId = reservationConfirmation.ConfirmedMedicalRecordsUserId,
                                ConfirmedSupervisorUserId = reservationConfirmation.ConfirmedSupervisorUserId,
                                RequestedTime = reservation.RequestedTime,
                                Status = reservationConfirmationStatus.Name,
                                StatusType = reservationConfirmationStatus.Id,
                                IsExtera = timing.IsExtraTiming,
                                PatientHaveInsurance = reservation.PatientHaveInsurance
                            };

            return await baseQuery.ToListAsync();
        }


        public async Task<ListResponseDto<ReservationDto>> GetPaginatedReservedList(PaginationDto paginationRequest, string noNezam, bool isExtera)
        {
            ListResponseDto<ReservationDto> responseDto = new ListResponseDto<ReservationDto>();


            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from reservation in Context.Reservations
                            join doctor in Context.Doctors on reservation.DoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on reservation.RoomCode equals room.Code
                            join reservationConfirmation in Context.ReservationConfirmations on reservation.Id equals reservationConfirmation.ReservationId
                            join reservationConfirmationStatus in Context.ReservationConfirmationStatuses on reservationConfirmation.StatusId equals reservationConfirmationStatus.Id
                            where !reservation.IsDeleted && reservation.IsActive &&
                            reservation.DoctorNoNezam.Equals(noNezam) &&
                            reservationConfirmation.IsActive &&
                            !reservationConfirmation.IsDeleted

                            select new ReservationDto
                            {
                                Id = reservation.Id,
                                TimingId = reservation.TimingId,
                                PatientName = reservation.PatientName,
                                PatientLastName = reservation.PatientLastName,
                                PatientNationalCode = reservation.PatientNationalCode,
                                PatientPhoneNumber = reservation.PatientPhoneNumber,
                                DoctorNoNezam = doctor.NoNezam,
                                DoctorName = doctor.Name,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                Description = reservation.Description,
                                RequestedDate = reservation.RequestedDate,
                                IsConfirmedByMedicalRecords = reservationConfirmation.IsConfirmedByMedicalRecords,
                                IsConfirmedBySupervisor = reservationConfirmation.IsConfirmedBySupervisor,
                                ConfirmedMedicalRecordsUserId = reservationConfirmation.ConfirmedMedicalRecordsUserId,
                                ConfirmedSupervisorUserId = reservationConfirmation.ConfirmedSupervisorUserId,
                                RequestedTime = reservation.RequestedTime,
                                Status = reservationConfirmationStatus.Name,
                                StatusType = reservationConfirmationStatus.Id
                            };
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                baseQuery = baseQuery.Where(u => u.DoctorNoNezam.Contains(paginationRequest.Searchkey) || u.DoctorName.Contains(paginationRequest.Searchkey));
            }

            var query = paginationRequest.FilterType == FilterType.Asc ?
                        baseQuery.OrderBy(u => u.Id) :
                        baseQuery.OrderByDescending(u => u.Id);
            responseDto.TotalCount = await baseQuery.CountAsync();


            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);
            responseDto.List = await pagedQuery.ToListAsync();
            return responseDto;
        }


        public async Task<ListResponseDto<ReservationDto>> GetReservationRejectionList(PaginationDto paginationRequest, string noNezam)
        {
            ListResponseDto<ReservationDto> responseDto = new ListResponseDto<ReservationDto>();

            var rejectionIds = new List<int> { 4, 5 };

            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from reservation in Context.Reservations
                            join doctor in Context.Doctors on reservation.DoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on reservation.RoomCode equals room.Code
                            join reservationConfirmation in Context.ReservationConfirmations on reservation.Id equals reservationConfirmation.ReservationId
                            join reservationConfirmationStatus in Context.ReservationConfirmationStatuses on reservationConfirmation.StatusId equals reservationConfirmationStatus.Id
                            join reservationRejectionAndCancellationReasons in Context.ReservationRejectionAndCancellationReasons on reservationConfirmation.ReservationRejectionAndCancellationReasonId equals reservationRejectionAndCancellationReasons.Id
                            where !reservation.IsDeleted && reservation.IsActive &&
                            reservation.DoctorNoNezam.Equals(noNezam) &&
                            reservationConfirmation.IsActive &&
                            !reservationConfirmation.IsDeleted && rejectionIds.Contains(reservationConfirmation.StatusId)
                            select new ReservationDto
                            {
                                Id = reservation.Id,
                                TimingId = reservation.TimingId,
                                PatientName = reservation.PatientName,
                                PatientLastName = reservation.PatientLastName,
                                PatientNationalCode = reservation.PatientNationalCode,
                                PatientPhoneNumber = reservation.PatientPhoneNumber,
                                DoctorNoNezam = doctor.NoNezam,
                                DoctorName = doctor.Name,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                Description = reservation.Description,
                                RequestedDate = reservation.RequestedDate,
                                IsConfirmedByMedicalRecords = reservationConfirmation.IsConfirmedByMedicalRecords,
                                IsConfirmedBySupervisor = reservationConfirmation.IsConfirmedBySupervisor,
                                ConfirmedMedicalRecordsUserId = reservationConfirmation.ConfirmedMedicalRecordsUserId,
                                ConfirmedSupervisorUserId = reservationConfirmation.ConfirmedSupervisorUserId,
                                RequestedTime = reservation.RequestedTime,
                                Status = reservationConfirmationStatus.Name,
                                StatusType = reservationConfirmationStatus.Id,
                                CancelationDescription = reservationConfirmation.RejectionAndCancellationAdditionalDescription,
                                ReservationCancelationReasonId = reservationRejectionAndCancellationReasons.Id,
                                ReservationCancelationReason = reservationRejectionAndCancellationReasons.Reason,
                                PatientHaveInsurance = reservation.PatientHaveInsurance
                            };
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                baseQuery = baseQuery.Where(u => u.DoctorNoNezam.Contains(paginationRequest.Searchkey) || u.DoctorName.Contains(paginationRequest.Searchkey));
            }

            var query = paginationRequest.FilterType == FilterType.Asc ?
                        baseQuery.OrderBy(u => u.Id) :
                        baseQuery.OrderByDescending(u => u.Id);

            responseDto.TotalCount = await baseQuery.CountAsync();
            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);
            responseDto.List = await pagedQuery.ToListAsync();
            return responseDto;
        }


        public async Task<ListResponseDto<ReservationDto>> GetPaginatedReservervationsList(PaginationDto paginationRequest, string operatorType, ReservationStatus status)
        {
            ListResponseDto<ReservationDto> responseDto = new ListResponseDto<ReservationDto>();


            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from reservation in Context.Reservations
                            join doctor in Context.Doctors on reservation.DoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on reservation.RoomCode equals room.Code
                            join reservationConfirmation in Context.ReservationConfirmations on reservation.Id equals reservationConfirmation.ReservationId
                            join reservationConfirmationStatus in Context.ReservationConfirmationStatuses on reservationConfirmation.StatusId equals reservationConfirmationStatus.Id
                            join timing in Context.Timings on reservation.TimingId equals timing.Id
                            where !reservation.IsDeleted && reservation.IsActive && reservationConfirmation.IsActive && !reservationConfirmation.IsDeleted
                            select new ReservationDto
                            {
                                Id = reservation.Id,
                                TimingId = reservation.TimingId,
                                PatientName = reservation.PatientName,
                                PatientLastName = reservation.PatientLastName,
                                PatientNationalCode = reservation.PatientNationalCode,
                                PatientPhoneNumber = reservation.PatientPhoneNumber,
                                DoctorNoNezam = doctor.NoNezam,
                                DoctorName = doctor.Name,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                Description = reservation.Description,
                                RequestedDate = reservation.RequestedDate,
                                IsConfirmedByMedicalRecords = reservationConfirmation.IsConfirmedByMedicalRecords,
                                IsConfirmedBySupervisor = reservationConfirmation.IsConfirmedBySupervisor,
                                ConfirmedMedicalRecordsUserId = reservationConfirmation.ConfirmedMedicalRecordsUserId,
                                ConfirmedSupervisorUserId = reservationConfirmation.ConfirmedSupervisorUserId,
                                RequestedTime = reservation.RequestedTime,
                                Status = reservationConfirmationStatus.Name,
                                StatusType = reservationConfirmationStatus.Id,
                                IsExtera = timing.IsExtraTiming,
                                PatientHaveInsurance = reservation.PatientHaveInsurance,
                            };
            if (operatorType == "Supervisor")
            {

                if (status == ReservationStatus.Approved)
                {
                    baseQuery = baseQuery.Where(x => x.IsExtera && x.StatusType == (int)ReservationConfirmationStatus.ApprovedBySupervisor);
                }
                else if (status == ReservationStatus.Rejected)
                {
                    baseQuery = baseQuery.Where(x => x.IsExtera && x.StatusType == (int)ReservationConfirmationStatus.RejectedBySupervisor);
                }
                else if (status == ReservationStatus.Pending)
                {
                    baseQuery = baseQuery.Where(x => x.IsExtera && x.StatusType == (int)ReservationConfirmationStatus.Pending);
                }
                else
                {
                    baseQuery = baseQuery.Where(x => x.IsExtera &&
                    x.StatusType != (int)ReservationConfirmationStatus.CancelledByMedicalRecord &&
                    x.StatusType != (int)ReservationConfirmationStatus.CancelledByDoctor &&
                    x.StatusType != (int)ReservationConfirmationStatus.CancelledBySupervisor
                    );
                }

            }
            else if (operatorType == "MedicalRecord")
            {
                if (status == ReservationStatus.Approved)
                {
                    baseQuery = baseQuery.Where(x =>
                       (x.IsExtera && x.IsConfirmedBySupervisor && x.IsConfirmedByMedicalRecords)
                       ||
                       (!x.IsExtera && x.StatusType == (int)ReservationConfirmationStatus.ApprovedByMedicalRecord));

                }
                else if (status == ReservationStatus.Rejected)
                {
                    baseQuery = baseQuery.Where(x =>

                       (x.IsExtera && x.IsConfirmedBySupervisor)
                       ||
                       (!x.IsExtera && x.StatusType == (int)ReservationConfirmationStatus.RejectedByMedicalRecord));

                }
                else if (status == ReservationStatus.Pending)
                {
                    baseQuery = baseQuery.Where(x =>

                       (x.IsExtera && x.IsConfirmedBySupervisor)
                       ||
                       (!x.IsExtera && x.StatusType == (int)ReservationConfirmationStatus.Pending));
                }
                else if (status == ReservationStatus.Extera)
                {
                    baseQuery = baseQuery.Where(x =>
                    x.IsExtera && x.IsConfirmedBySupervisor);
                }
                else
                {
                    var cancelledStatusTypes = new[]
                    {
                    (int)ReservationConfirmationStatus.CancelledByMedicalRecord,
                    (int)ReservationConfirmationStatus.CancelledByDoctor,
                    (int)ReservationConfirmationStatus.CancelledBySupervisor
                    };

                    baseQuery = baseQuery.Where(x =>
                        (x.IsExtera && x.IsConfirmedBySupervisor && !cancelledStatusTypes.Contains(x.StatusType))
                        ||
                        (!x.IsExtera && !cancelledStatusTypes.Contains(x.StatusType))
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                baseQuery = baseQuery.Where(u => u.DoctorNoNezam.Contains(paginationRequest.Searchkey) || u.DoctorName.Contains(paginationRequest.Searchkey));
            }

            var query = paginationRequest.FilterType == FilterType.Asc ?
                        baseQuery.OrderBy(u => u.Id) :
                        baseQuery.OrderByDescending(u => u.Id);


            responseDto.TotalCount = await baseQuery.CountAsync();
            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);
            responseDto.List = await pagedQuery.ToListAsync();
            return responseDto;
        }

        public async Task<Reservation?> GetReservationById(Guid id)
        {
            return await Context.Reservations.FirstOrDefaultAsync(u => u.Id.Equals(id) && !u.IsDeleted && u.IsActive);
        }
        public async Task<ReservationConfirmation?> GetReservationConfirmationById(Guid id)
        {
            return await Context.ReservationConfirmations.FirstOrDefaultAsync(u => u.Id.Equals(id) && !u.IsDeleted && u.IsActive);
        }

        public async Task<ReservationConfirmation?> GetReservationConfirmationByReservationId(Guid id)
        {
            return await Context.ReservationConfirmations.Include(x => x.ReservationConfirmationType).FirstOrDefaultAsync(u => u.ReservationId.Equals(id) && !u.IsDeleted && u.IsActive);
        }

        public async Task<IEnumerable<ReservationRejectionAndCancellationReason>> GetReservationRejectionReasonByType(RejectionReasonType type)
        {
            return await Context.ReservationRejectionAndCancellationReasons.Where(u => u.RejectionReasonType == type && !u.IsDeleted && u.IsActive).ToListAsync();
        }

        public async Task<ListResponseDto<ReservationDto>> GetExteraReservationList(PaginationDto paginationRequest)
        {

            ListResponseDto<ReservationDto> responseDto = new ListResponseDto<ReservationDto>();

            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from reservation in Context.Reservations
                            join doctor in Context.Doctors on reservation.DoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on reservation.RoomCode equals room.Code
                            join reservationConfirmation in Context.ReservationConfirmations on reservation.Id equals reservationConfirmation.ReservationId
                            join reservationConfirmationStatus in Context.ReservationConfirmationStatuses on reservationConfirmation.StatusId equals reservationConfirmationStatus.Id
                            join timing in Context.Timings on reservation.TimingId equals timing.Id
                            join reservationRejectionAndCancellationReasons in Context.ReservationRejectionAndCancellationReasons on reservationConfirmation.ReservationRejectionAndCancellationReasonId equals reservationRejectionAndCancellationReasons.Id
                            where !reservation.IsDeleted && reservation.IsActive && timing.IsExtraTiming == true
                            select new ReservationDto
                            {
                                Id = reservation.Id,
                                TimingId = reservation.TimingId,
                                PatientName = reservation.PatientName,
                                PatientLastName = reservation.PatientLastName,
                                PatientNationalCode = reservation.PatientNationalCode,
                                PatientPhoneNumber = reservation.PatientPhoneNumber,
                                DoctorNoNezam = doctor.NoNezam,
                                DoctorName = doctor.Name,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                Description = reservation.Description,
                                RequestedDate = reservation.RequestedDate,
                                IsConfirmedByMedicalRecords = reservationConfirmation.IsConfirmedByMedicalRecords,
                                IsConfirmedBySupervisor = reservationConfirmation.IsConfirmedBySupervisor,
                                ConfirmedMedicalRecordsUserId = reservationConfirmation.ConfirmedMedicalRecordsUserId,
                                ConfirmedSupervisorUserId = reservationConfirmation.ConfirmedSupervisorUserId,
                                RequestedTime = reservation.RequestedTime,
                                Status = reservationConfirmationStatus.Name,
                                StatusType = reservationConfirmationStatus.Id,
                                //CancelationDescription = reservation.CancelationDescription,
                                ReservationCancelationReasonId = reservationRejectionAndCancellationReasons.Id,
                                ReservationCancelationReason = reservationRejectionAndCancellationReasons.Reason,
                                PatientHaveInsurance = reservation.PatientHaveInsurance
                            };
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                baseQuery = baseQuery.Where(u => u.DoctorNoNezam.Contains(paginationRequest.Searchkey) || u.DoctorName.Contains(paginationRequest.Searchkey));
            }

            var query = paginationRequest.FilterType == FilterType.Asc ?
                        baseQuery.OrderBy(u => u.Id) :
                        baseQuery.OrderByDescending(u => u.Id);

            responseDto.TotalCount = await baseQuery.CountAsync();
            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);
            responseDto.List = await pagedQuery.ToListAsync();
            return responseDto;
        }

        public async Task<IEnumerable<Reservation?>> GetReservationsByTimingId(Guid timingId)
        {
            return await Context.Reservations.Include(x => x.ReservationConfirmation).Where(x => x.TimingId.Equals(timingId) && !x.IsDeleted && x.IsActive).ToListAsync();
        }
    }
}
