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
using SurgeryRoomScheduler.Domain.Dtos.Reservation;

namespace SurgeryRoomScheduler.Data.Repositories
{
    public class TimingRepository : Repository<Timing>, ITimingRepository
    {
        public TimingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckTimingExist(string noNezam, long roomCode, DateOnly date, TimeOnly eDate, TimeOnly sDate)
        {
            return await Context.Timings
                 .AnyAsync(x => x.AssignedDoctorNoNezam.Equals(noNezam) &&
                                x.AssignedRoomCode == roomCode &&
                                x.ScheduledDate == date &&
                                x.ScheduledStartTime <eDate && // Check for time overlap
                                x.ScheduledEndTime >sDate); // Check for time overlap
        }
        public async Task<ListResponseDto<TimingDto>> GetExteraTimingListByRoomCode(PaginationDto paginationRequest, long roomCode)
        {
            ListResponseDto<TimingDto> responseDto = new ListResponseDto<TimingDto>();


            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            IQueryable<TimingDto> timings = new List<TimingDto>().AsQueryable();
            if (roomCode == 00) // superviser of medical Record User
            {
                timings = from timing in Context.Timings
                          join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                          where !timing.IsDeleted && timing.IsActive
                          && timing.IsExtraTiming
                          && timing.ScheduledDate >= DateOnly.FromDateTime(DateTime.Now) && timing.ScheduledDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(3))
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
                              IsActive = timing.IsActive,
                          };
            }
            else
            {
                timings = from timing in Context.Timings
                          join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                          where !timing.IsDeleted && timing.IsActive && timing.AssignedRoomCode == roomCode && timing.IsExtraTiming
                          && timing.ScheduledDate >= DateOnly.FromDateTime(DateTime.Now) && timing.ScheduledDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(3))
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
                              IsActive = timing.IsActive,
                          };
            }


            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                timings = timings.Where(u => u.DoctorNoNezam.Contains(paginationRequest.Searchkey) || u.DoctorName.Contains(paginationRequest.Searchkey));
            }
            var query = paginationRequest.FilterType == FilterType.Desc ?
                        timings.OrderBy(u => u.ScheduledDate) :
                        timings.OrderByDescending(u => u.ScheduledDate);


            responseDto.TotalCount = await timings.CountAsync();


            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);

            var timingList = await pagedQuery.ToListAsync();
            var timingIds = timingList.Select(x => x.Id).ToList();

            var reservations = Context.Reservations.Where(x => timingIds.Contains(x.TimingId) && x.RoomCode == roomCode && x.IsActive && !x.IsDeleted)
            .Select(x => new ReservationInfo { RequestedTime = x.RequestedTime, TimingId = x.TimingId })
            .ToList();

            foreach (var item in timingList)
            {
                item.RemainingTime = item.ScheduledDuration - reservations.Where(x => x.TimingId == item.Id)
                                    .Select(x => x.RequestedTime)
                                    .Aggregate(TimeSpan.Zero, (sum, next) => sum + next);
            }
            responseDto.List = timingList;
            return responseDto;
        }


        public async Task<IEnumerable<TimingDto>> GetDoctorTimingByRoomIdAndDate(long roomCode, string noNezam, DateTime sDate, DateTime eDate)
        {
            var timingsData = await (
               from timing in Context.Timings
               join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
               join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
               where timing.AssignedRoomCode == roomCode
              && !timing.IsDeleted
              && timing.IsActive
              && timing.ScheduledDate >= DateOnly.FromDateTime(sDate.Date)
              && timing.ScheduledDate <= DateOnly.FromDateTime(eDate.Date)
              && timing.AssignedDoctorNoNezam.Equals(noNezam)
               select new
               {
                   Timing = timing,
                   Doctor = doctor,
                   Room = room,
                   Reservations = Context.Reservations
                .Where(x => x.TimingId.Equals(timing.Id))
                .Select(x => x.RequestedTime)
                .ToList() // Retrieve the reservations for each timing
               }
                    ).ToListAsync();

            // Perform the remaining calculations in-memory
            var timings = timingsData.Select(t => new TimingDto
            {
                Id = t.Timing.Id,
                DoctorNoNezam = t.Doctor.NoNezam,
                DoctorName = t.Doctor.Name,
                RoomName = t.Room.Name,
                RoomCode = t.Room.Code,
                ScheduledDate = t.Timing.ScheduledDate,
                ScheduledStartTime = t.Timing.ScheduledStartTime,
                ScheduledEndTime = t.Timing.ScheduledEndTime,
                ScheduledDuration = t.Timing.ScheduledDuration,
                ScheduledDate_Shamsi = t.Timing.ScheduledDate_Shamsi,
                CreatedDate = t.Timing.CreatedDate,
                CreatedDate_Shamsi = t.Timing.CreatedDate_Shamsi,
                IsDeleted = t.Timing.IsDeleted,
                IsActive = t.Timing.IsActive,
                RemainingTime = t.Timing.ScheduledDuration - t.Reservations.Aggregate(TimeSpan.Zero, (sum, next) => sum + next)
            }).ToList();

            return timings;

        }

        public async Task<ListResponseDto<TimingDto>> GetPaginatedTimingList(PaginationDto paginationRequest)
        {
            ListResponseDto<TimingDto> responseDto = new ListResponseDto<TimingDto>();



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

            responseDto.TotalCount = await baseQuery.CountAsync();
            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);
            responseDto.List = await pagedQuery.ToListAsync();
            return responseDto;
        }

        public async Task<ListResponseDto<TimingDto>> GetPaginatedTimingListByRoomAndDate(PaginationDto paginationRequest, long roomCode, DateOnly date)
        {
            ListResponseDto<TimingDto> responseDto = new ListResponseDto<TimingDto>();


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

            responseDto.TotalCount = await baseQuery.CountAsync();
            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);
            responseDto.List = await pagedQuery.ToListAsync();
            return responseDto;
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
            var CancelledIds = new List<int> { 6, 7, 8 };



            GetExteraTimingDto timingDto = new GetExteraTimingDto();
            // 3 days Later Not Used Timings
            var notReservedTimings = Context.Timings
            .Where(x => x.ScheduledDate <= date && !x.IsExtraTiming && !x.IsDeleted && x.IsActive)
            .GroupJoin(


            Context.Reservations.Include(x => x.ReservationConfirmation).Where(r => r.IsActive && !r.IsDeleted &&  !CancelledIds.Contains(r.ReservationConfirmation.StatusId)),



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
            // 3 days Later Not Fully Reserved Timings
            var notFullyReservedTimings = await Context.Timings
            .Where(x => x.ScheduledDate <= date && !x.IsExtraTiming && !x.IsDeleted && x.IsActive)
            .GroupJoin(
                Context.Reservations
                  .Include(x => x.ReservationConfirmation)
                                .Where(x => !CancelledIds.Contains(x.ReservationConfirmation.StatusId) &&
                                            x.IsActive &&
                                            !x.IsDeleted && x.UsageTime > TimeSpan.FromMinutes(20)),
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

        public async Task<ListResponseDto<GetTimingDto>> GetDoctorDayOffList(PaginationDto paginationRequest, string noNezam, DateOnly startDate, DateOnly endDate)
        {
            ListResponseDto<GetTimingDto> responseDto = new ListResponseDto<GetTimingDto>();
            var CancelledIds = new List<int> { 6, 7, 8 };

            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var baseQuery = from timing in Context.Timings
                            join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
                            join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
                            where !timing.IsDeleted && timing.IsActive && doctor.NoNezam.Equals(noNezam)
                            && timing.ScheduledDate == startDate //add End Date Soon
                            select new GetTimingDto
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
                                IsActive = timing.IsActive,
                                IsExtera = timing.IsExtraTiming,
                                Reservations = Context.Reservations.Include(x => x.ReservationConfirmation).ThenInclude(x => x.Status).Where(x => !CancelledIds.Contains(x.ReservationConfirmation.StatusId) &&
                                x.TimingId.Equals(timing.Id) &&
                                x.IsActive &&
                                !x.IsDeleted)

                                .Select(x => new ReservationDto
                                {
                                    TimingId = x.TimingId,
                                    PatientName = x.PatientName,
                                    PatientHaveInsurance = x.PatientHaveInsurance,
                                    PatientNationalCode = x.PatientNationalCode,
                                    PatientPhoneNumber = x.PatientPhoneNumber,
                                    PatientLastName = x.PatientLastName,
                                    RequestedTime = x.RequestedTime,
                                    RequestedDate = x.RequestedDate,
                                    Status = x.ReservationConfirmation.Status.Name,
                                }).ToList()
                                ,
                                IsReserved = Context.Reservations
                                .Include(x => x.ReservationConfirmation)
                                .Where(x =>!CancelledIds.Contains(x.ReservationConfirmation.StatusId) &&
                                            x.TimingId.Equals(timing.Id) &&
                                            x.IsActive &&
                                            !x.IsDeleted)
                                .Any()
                                //RemainingTime = t.Timing.ScheduledDuration - t.Reservations.Aggregate(TimeSpan.Zero, (sum, next) => sum + next)
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

    }
}
