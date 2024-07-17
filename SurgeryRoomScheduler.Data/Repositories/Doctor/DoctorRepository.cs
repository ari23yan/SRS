using Microsoft.EntityFrameworkCore;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Entities.Account;
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
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> DeleteDoctors()
        {
            try
            {
                await Context.Doctors.ExecuteDeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> DeleteDoctorsAssignedRooms()
        {
            try
            {
                await Context.DoctorRooms.ExecuteDeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> DeleteInsurances()
        {
            try
            {
                await Context.Insurances.ExecuteDeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> DeleteSurgeryNames()
        {
            try
            {
                await Context.SurgeryNames.ExecuteDeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> DeleteRooms()
        {
            try
            {
                await Context.Rooms.ExecuteDeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<IEnumerable<Room>> GetDoctorRooms(string noNezam)
        {
            var RoomIds = await Context.DoctorRooms
                               .Where(x => x.NoNezam.Contains(noNezam))
                               .Select(x => x.RoomCode)
                               .Distinct()
                               .ToListAsync();

            var doctorRooms = await Context.Rooms
                                           .Where(x => RoomIds.Contains(x.Code.Value))
                                           .ToListAsync();

            return doctorRooms;
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsList(long roomCode, string searchKey)
        {
            if(roomCode == 0)
            {
                var query = Context.Doctors.Where(x => x.IsActive.Value).Take(50);
                if (!string.IsNullOrEmpty(searchKey))
                {
                    query = query.Where(x => x.NoNezam.Contains(searchKey) || x.FullName.Contains(searchKey)).Take(50);
                }
                return await query.ToListAsync();
            }
            else
            {
                var rooms = Context.DoctorRooms.Where(x => x.RoomCode == roomCode);
                var doctorIds = rooms.Select(x => x.NoNezam);
                var query = Context.Doctors.Where(x => doctorIds.Contains(x.NoNezam) && x.IsActive.Value).Take(50);
                if (!string.IsNullOrEmpty(searchKey))
                {
                    query = query.Where(x => x.NoNezam.Contains(searchKey) || x.FullName.Contains(searchKey)).Take(50);
                }
                return await query.ToListAsync();
            }
           
        }

        public async Task<IEnumerable<Room>> GetRoomsList(string searchKey)
        {
            var query = Context.Rooms.Where(x => x.IsActive.Value);
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.Name.Contains(searchKey));
            }
            return await query.ToListAsync();
        }

        public async Task<ListResponseDto<Doctor>> GetDoctorsListPaginated(PaginationDto paginationRequest)
        {
            //ListResponseDto<Room> responseDto = new ListResponseDto<Room>();

            //var rooms = Context.DoctorRooms.Where(x => x.RoomCode == roomCode);
            //var doctorIds = rooms.Select(x => x.NoNezam);
            //var query = Context.Doctors.Where(x => doctorIds.Contains(x.NoNezam) && x.IsActive.Value).Take(50);
            //if (!string.IsNullOrEmpty(searchKey))
            //{
            //    query = query.Where(x => x.NoNezam.Contains(searchKey) || x.FullName.Contains(searchKey)).Take(50);
            //}
            //return await query.ToListAsync();



            ListResponseDto<Doctor> responseDto = new ListResponseDto<Doctor>();

            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            IQueryable<Doctor> query = Context.Doctors.Where(u => u.IsActive.HasValue && u.IsActive.Value);
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                query = query.Where(u => u.FullName.Contains(paginationRequest.Searchkey) || u.NoNezam.Contains(paginationRequest.Searchkey));
            }
            query = paginationRequest.FilterType == FilterType.Asc ?
                query.OrderBy(u => u.Id) :
                query.OrderByDescending(u => u.Id);


            responseDto.TotalCount = await query.CountAsync();
            var pagedQuery = query.Skip(skipCount).Take(paginationRequest.PageSize);
            responseDto.List = await pagedQuery.ToListAsync();
            return responseDto;

        }
    }
}
