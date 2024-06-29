using Microsoft.EntityFrameworkCore;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<ReservationDto>> GetPaginatedReservedList(PaginationDto paginationRequest, string noNezam)
        {
            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from reservation in Context.Reservations
                            join doctor in Context.Doctors on reservation.DoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on reservation.RoomCode equals room.Code
                            where !reservation.IsDeleted && reservation.IsActive && reservation.DoctorNoNezam.Equals(noNezam)
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
                                //IsConfirmedByMedicalRecords = reservation.IsConfirmedByMedicalRecords,
                                //ConfirmedMedicalRecordsUserId = reservation.ConfirmedMedicalRecordsUserId,
                                //IsConfirmedBySupervisor = reservation.IsConfirmedBySupervisor,
                                //ConfirmedSupervisorUserId = reservation.ConfirmedSupervisorUserId,
                                RequestedTime = reservation.RequestedTime,
                                //Status = reservation.Status
                            };
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                baseQuery = baseQuery.Where(u => u.DoctorNoNezam.Contains(paginationRequest.Searchkey) || u.DoctorName.Contains(paginationRequest.Searchkey));
            }

            var query = paginationRequest.FilterType == FilterType.Asc ?
                        baseQuery.OrderBy(u => u.Id) :
                        baseQuery.OrderByDescending(u => u.Id);

            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);

            return await pagedQuery.ToListAsync();
        }

        public async Task<IEnumerable<ReservationDto>> GetPaginatedReservervationsList(PaginationDto paginationRequest)
        {
            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from reservation in Context.Reservations
                            join doctor in Context.Doctors on reservation.DoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on reservation.RoomCode equals room.Code
                            where !reservation.IsDeleted && reservation.IsActive
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
                                //IsConfirmedByMedicalRecords = reservation.IsConfirmedByMedicalRecords,
                                //ConfirmedMedicalRecordsUserId = reservation.ConfirmedMedicalRecordsUserId,
                                //IsConfirmedBySupervisor = reservation.IsConfirmedBySupervisor,
                                //ConfirmedSupervisorUserId = reservation.ConfirmedSupervisorUserId,
                                //RequestedTime = reservation.RequestedTime,
                                //Status = reservation.Status
                            };
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                baseQuery = baseQuery.Where(u => u.DoctorNoNezam.Contains(paginationRequest.Searchkey) || u.DoctorName.Contains(paginationRequest.Searchkey));
            }

            var query = paginationRequest.FilterType == FilterType.Asc ?
                        baseQuery.OrderBy(u => u.Id) :
                        baseQuery.OrderByDescending(u => u.Id);

            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);

            return await pagedQuery.ToListAsync();
        }

        public async Task<Reservation?> GetReservationById(Guid id)
        {
            return await Context.Reservations.FirstOrDefaultAsync(u => u.Id.Equals(id) && !u.IsDeleted && u.IsActive);
        }
    }
}
