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
                                x.ScheduledDate < request.Date &&
                                x.ScheduledStartTime < request.EndTime &&
                                x.ScheduledEndTime > request.StartTime);
        }

        public async Task<IEnumerable<TimingDto>> GetDoctorTimingByRoomIdAndDate(long roomCode,string noNezam, DateTime sDate, DateTime eDate)
        {
            var timings = await(
               from timing in Context.Timings
               join doctor in Context.Doctors on timing.AssignedDoctorNoNezam equals doctor.NoNezam
               join room in Context.Rooms on timing.AssignedRoomCode equals room.Code
               where timing.AssignedRoomCode == roomCode
                   && !timing.IsDeleted
                   && timing.IsActive
                   && timing.ScheduledDate == DateOnly.FromDateTime(sDate.Date)
                   //&& timing.ScheduledEndDate.Date <= eDate.Date
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
                            where !timing.IsDeleted && timing.IsActive && timing.AssignedRoomCode.Equals(roomCode)  && timing.ScheduledDate == date
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

        public Task<ResponseDto<IEnumerable<TimingDto>>> GetTimingListByDate(DateOnly date)
        {
            throw new NotImplementedException();
        }
    }
}
