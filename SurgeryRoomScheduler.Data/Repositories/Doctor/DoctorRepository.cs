using Microsoft.EntityFrameworkCore;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Domain.Entities.General;
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
            catch(Exception ex)
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

        public async Task<IEnumerable<Doctor>> GetDoctorsList(string searchKey)
        {
            var query = Context.Doctors.Where(x => x.IsActive.Value).Take(50);
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.NoNezam.Contains(searchKey) || x.FullName.Contains(searchKey)).Take(50);
            }
            return await query.ToListAsync();
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
    }
}
