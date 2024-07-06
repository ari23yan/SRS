using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Domain.Interfaces;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SurgeryRoomScheduler.Data.Repositories
{
    public class TimingRepository : Repository<Timing>, ITimingRepository
    {
        public TimingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckTimingExist(AddTimingDto request)
        {
            return await Context.Timings
                 .AnyAsync(x => x.AssignedDoctorNoNezam.Equals(request.NoNezam) &&
                                x.AssignedRoomCode.Equals(request.RoomCode) &&
                                x.ScheduledDate == request.Date &&
                                x.ScheduledStartTime < request.EndTime && // Check for time overlap
                                x.ScheduledEndTime > request.StartTime); // Check for time overlap
        }
        public async Task<IEnumerable<TimingDto>> GetDoctorTimingByRoomIdAndDate(long roomCode, string noNezam, DateTime sDate, DateTime eDate)
        {
            var timings = await (
               from timing in Context.Timings
               join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
               join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
               where timing.AssignedRoomCode == roomCode
                   && !timing.IsDeleted
                   && timing.IsActive
                   && timing.ScheduledDate >= DateOnly.FromDateTime(sDate.Date)
                   && timing.ScheduledDate <= DateOnly.FromDateTime(eDate.Date)
                   && timing.AssignedDoctorNoNezam.Equals(noNezam)
               select new TimingDto
               {
                   Id = timing.Id,
                   DoctorNoNezam = doctor.NoNezam,
                   DoctorName = doctor.Name,
                   RoomName = room.Name,
                   RoomCode = room.Code,
                   ScheduledDate = timing.ScheduledDate,
                   ScheduledStartTime = timing.ScheduledStartTime,
                   ScheduledEndTime = timing.ScheduledEndTime,
                   ScheduledDuration = timing.ScheduledDuration,
                   ScheduledDate_Shamsi = timing.ScheduledDate_Shamsi,
                   CreatedDate = timing.CreatedDate,
                   CreatedDate_Shamsi = timing.CreatedDate_Shamsi,
                   IsDeleted = timing.IsDeleted,
                   IsActive = timing.IsActive
               }
           ).ToListAsync();

            return timings;
        }

        public async Task<IEnumerable<TimingDto>> GetPaginatedTimingList(PaginationDto paginationRequest)
        {
            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from timing in Context.Timings
                            join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                            where !timing.IsDeleted && timing.IsActive
                            select new TimingDto
                            {
                                Id = timing.Id,
                                DoctorNoNezam = doctor.NoNezam,
                                DoctorName = doctor.Name,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                ScheduledDate = timing.ScheduledDate,
                                ScheduledStartTime = timing.ScheduledStartTime,
                                ScheduledEndTime = timing.ScheduledEndTime,
                                ScheduledDuration = timing.ScheduledDuration,
                                ScheduledDate_Shamsi = timing.ScheduledDate_Shamsi,
                                CreatedDate = timing.CreatedDate,
                                CreatedDate_Shamsi = timing.CreatedDate_Shamsi,
                                IsDeleted = timing.IsDeleted,
                                IsActive = timing.IsActive
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

        public async Task<IEnumerable<TimingDto>> GetPaginatedTimingListByRoomAndDate(PaginationDto paginationRequest, long roomCode, DateOnly date)
        {
            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from timing in Context.Timings
                            join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                            where !timing.IsDeleted && timing.IsActive && timing.AssignedRoomCode.Equals(roomCode) && timing.ScheduledDate == date
                            && timing.ScheduledDate <= date
                            select new TimingDto
                            {
                                Id = timing.Id,
                                DoctorNoNezam = doctor.NoNezam,
                                DoctorName = doctor.Name,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                ScheduledDate = timing.ScheduledDate,
                                ScheduledStartTime = timing.ScheduledStartTime,
                                ScheduledEndTime = timing.ScheduledEndTime,
                                ScheduledDuration = timing.ScheduledDuration,
                                ScheduledDate_Shamsi = timing.ScheduledDate_Shamsi,
                                CreatedDate = timing.CreatedDate,
                                CreatedDate_Shamsi = timing.CreatedDate_Shamsi,
                                IsDeleted = timing.IsDeleted,
                                IsActive = timing.IsActive
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

        public async Task<Timing?> GetTimingById(Guid timingId)
        {
            return await Context.Timings.FirstOrDefaultAsync(u => u.Id.Equals(timingId) && !u.IsDeleted && u.IsActive);
        }

        public async Task<IEnumerable<TimingDto>> GetTimingByRoomIdAndDate(long roomCode, DateTime sDate, DateTime eDate)
        {
            var timings = await (
                from timing in Context.Timings
                join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
                join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                where timing.AssignedRoomCode == roomCode
                    && !timing.IsDeleted
                    && timing.IsActive
                    && timing.ScheduledDate >= DateOnly.FromDateTime(sDate)
                    && timing.ScheduledDate <= DateOnly.FromDateTime(eDate)
                select new TimingDto
                {
                    Id = timing.Id,
                    DoctorNoNezam = doctor.NoNezam,
                    DoctorName = doctor.Name,
                    RoomName = room.Name,
                    RoomCode = room.Code,
                    ScheduledDate = timing.ScheduledDate,
                    ScheduledStartTime = timing.ScheduledStartTime,
                    ScheduledEndTime = timing.ScheduledEndTime,
                    ScheduledDuration = timing.ScheduledDuration,
                    ScheduledDate_Shamsi = timing.ScheduledDate_Shamsi,
                    CreatedDate = timing.CreatedDate,
                    CreatedDate_Shamsi = timing.CreatedDate_Shamsi,
                    IsDeleted = timing.IsDeleted,
                    IsActive = timing.IsActive
                }
            ).ToListAsync();

            return timings;
        }

        public async Task<TimingDto> GetTimingDetailByTimingId(Guid timingId)
        {
            var timingDto = await (
                from timing in Context.Timings
                join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
                join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                where timing.Id.Equals(timingId) && !timing.IsDeleted && timing.IsActive
                select new TimingDto
                {
                    Id = timing.Id,
                    DoctorNoNezam = doctor.NoNezam,
                    DoctorName = doctor.Name,
                    RoomName = room.Name,
                    RoomCode = room.Code,
                    ScheduledDate = timing.ScheduledDate,
                    ScheduledStartTime = timing.ScheduledStartTime,
                    ScheduledEndTime = timing.ScheduledEndTime,
                    ScheduledDuration = timing.ScheduledDuration,
                    ScheduledDate_Shamsi = timing.ScheduledDate_Shamsi,
                    CreatedDate = timing.CreatedDate,
                    CreatedDate_Shamsi = timing.CreatedDate_Shamsi,
                    IsDeleted = timing.IsDeleted,
                    IsActive = timing.IsActive
                }
            ).FirstOrDefaultAsync();

            return timingDto;
        }

        public async Task<GetExteraTimingDto> GetExteraTimingListByDate(DateOnly date)
        {
            GetExteraTimingDto timingDto = new GetExteraTimingDto();
            // 3 days Ago Not Used Timings
            var notReservedTimings = Context.Timings
            .Where(x => x.ScheduledDate <= date && !x.IsExtraTiming && !x.IsDeleted && x.IsActive)
            .GroupJoin(
            Context.Reservations.Where(r => r.IsActive && !r.IsDeleted && !r.IsCanceled),
            timing => timing.Id,
            reservation => reservation.TimingId,
            (timing, reservations) => new { timing, reservations }
            )
            .SelectMany(
            x => x.reservations.DefaultIfEmpty(),
            (x, reservation) => new { x.timing, reservation }
            )
            .Where(x => x.reservation == null)
            .Select(x => x.timing).ToList();
            timingDto.UnreservedTimings = notReservedTimings;
            // 3 days Ago Not Fully Reserved Timings
            var notFullyReservedTimings = await Context.Timings
            .Where(x => x.ScheduledDate <= date && !x.IsExtraTiming && !x.IsDeleted && x.IsActive)
            .GroupJoin(
                Context.Reservations
                    .Where(r => r.IsActive && !r.IsDeleted && !r.IsCanceled && r.UsageTime > TimeSpan.FromMinutes(20)),
                timing => timing.Id,
                reservation => reservation.TimingId,
                (timing, reservations) => new { timing, reservations }
            )
            .SelectMany(
                x => x.reservations.DefaultIfEmpty(),
                (x, reservation) => new { x.timing, reservation }
            )
            .Where(x => x.reservation != null) 
            .Select(x => new NotFullyReservedTimingsDto
            {
                TimingId = x.timing.Id,
                UsageTime = x.reservation.UsageTime,
                AssignedDoctorNoNezam = x.timing.AssignedDoctorNoNezam,
                CreatedDate_Shamsi = x.timing.CreatedDate_Shamsi,
                AssignedRoomCode = x.timing.AssignedRoomCode,
                IsExtraTiming = x.timing.IsExtraTiming,
                ScheduledDate = x.timing.ScheduledDate,
                ScheduledDate_Shamsi = x.timing.ScheduledDate_Shamsi,
                ScheduledDuration = x.timing.ScheduledDuration,
                ScheduledEndTime = x.timing.ScheduledEndTime,
                ScheduledStartTime = x.timing.ScheduledStartTime
            })
            .ToListAsync();
            timingDto.NotFullyReservedTimings = notFullyReservedTimings;
            return timingDto;
        }

        public async Task<IEnumerable<TimingDto>> GetExteraTimingListByRoomCode(PaginationDto paginationRequest, long roomCode)
        {
            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from timing in Context.Timings
                            join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                            where !timing.IsDeleted && timing.IsActive && timing.AssignedRoomCode == roomCode && timing.IsExtraTiming
                            select new TimingDto
                            {
                                Id = timing.Id,
                                RoomName = room.Name,
                                RoomCode = room.Code,
                                ScheduledDate = timing.ScheduledDate,
                                ScheduledStartTime = timing.ScheduledStartTime,
                                ScheduledEndTime = timing.ScheduledEndTime,
                                ScheduledDuration = timing.ScheduledDuration,
                                ScheduledDate_Shamsi = timing.ScheduledDate_Shamsi,
                                CreatedDate = timing.CreatedDate,
                                CreatedDate_Shamsi = timing.CreatedDate_Shamsi,
                                IsDeleted = timing.IsDeleted,
                                IsActive = timing.IsActive
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
    }
}
