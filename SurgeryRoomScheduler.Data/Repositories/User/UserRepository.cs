using Microsoft.EntityFrameworkCore;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurgeryRoomScheduler.Domain.Entities.Account;
using System.Reflection;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.Permission;
using SurgeryRoomScheduler.Domain.Entities.Common;

namespace SurgeryRoomScheduler.Data.Repositories.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<bool> CheckUserExist(string loginIdentifier)
        {
            return await Context.Users
            .AnyAsync(x => (x.PhoneNumber.Equals(loginIdentifier) || x.Username.Equals(loginIdentifier)) && !x.IsDeleted && x.IsActive);
        }

        public async Task<bool> CheckUserHavePermission(Guid roleId, Guid permissionId)
        {
            return await Context.RolePermissions.AnyAsync(x => x.RoleId.Equals(roleId) && x.PermissionId.Equals(permissionId));
        }

        public async Task<IEnumerable<User>> GetPaginatedUsersList(PaginationDto paginationRequest)
        {
            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            IQueryable<User> query = Context.Users.Include(x => x.Role).Where(u => !u.IsDeleted && u.IsActive);
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                query = query.Where(u => u.LastName.Contains(paginationRequest.Searchkey));
            }
            query = paginationRequest.FilterType == FilterType.Asc ?
                query.OrderBy(u => u.Id) :
                query.OrderByDescending(u => u.Id);
            return await query.Skip(skipCount).Take(paginationRequest.PageSize).ToListAsync();
        }



        public async Task<User?> GetUserDetailById(Guid userId)
        {
            return await Context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ThenInclude(p => p.PermissionGroup_Permissions)
                .ThenInclude(pg => pg.PermissionGroup)
                .Include(u => u.UserDetail)
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(u => u.Id.Equals(userId) && !u.IsDeleted && u.IsActive);
        }


        public async Task<Domain.Entities.Account.User?> GetUserAndOtpByMobile(string mobile)
        {
            return await Context.Users.Include(x=>x.Role).ThenInclude(x=>x.RoleMenus).Include(x => x.Otps).Include(x => x.UserDetail).FirstOrDefaultAsync(x => x.PhoneNumber.Equals(mobile) && !x.IsDeleted && x.IsActive);
        }
        public async Task<Domain.Entities.Account.User?> GetUserByMobile(string mobile)
        {
            return await Context.Users.Include(x => x.Otps).Include(x => x.UserDetail).FirstOrDefaultAsync(x => x.PhoneNumber.Equals(mobile) && !x.IsDeleted && x.IsActive);
        }

        public async Task<Domain.Entities.Account.User?> GetUserByUsername(string userName)
        {
            return await Context.Users.Include(x => x.Otps).Include(x => x.UserDetail).FirstOrDefaultAsync(x => x.Username.Equals(userName) && !x.IsDeleted && x.IsActive);
        }

        public async Task<User?> GetUserByUserId(Guid userId)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId) && !x.IsDeleted && x.IsActive);
        }

        public async Task<User?> GetUserWithDetailsById(Guid userId)
        {
            return await Context.Users.Include(x => x.UserDetail).FirstOrDefaultAsync(x => x.Id.Equals(userId) && !x.IsDeleted);
        }
        public async Task<User?> GetUserWithRolesByUserId(Guid userId)
        {
            return await Context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id.Equals(userId) && !x.IsDeleted);
        }

        public async Task<bool> CheckUserExistByPhoneMumber(string phoneNumber)
        {
            return await Context.Users.AnyAsync(x => x.PhoneNumber.Equals(phoneNumber) && !x.IsDeleted);
        }

        public async Task<bool> CheckUserExistByUsername(string username)
        {
            return await Context.Users.AnyAsync(x => x.Username.Equals(username) && !x.IsDeleted);
        }

        public async Task<IEnumerable<Role>> GetRolesList()
        {
            return await Context.Roles.Where(x => !x.IsDeleted && x.IsActive).ToListAsync();
        }

        public async Task<Role?> GetRoleById(Guid roleId)
        {
            return await Context.Roles.FirstOrDefaultAsync(u => u.Id.Equals(roleId) && !u.IsDeleted && u.IsActive);
        }

        public async Task<Role?> GetRoleWithDetailById(Guid roleId)
        {
            return await Context.Roles
             .Include(u => u.RoleMenus)
             .Include(u => u.RolePermissions)
             .ThenInclude(rp => rp.Permission)
             .ThenInclude(p => p.PermissionGroup_Permissions)
             .ThenInclude(pg => pg.PermissionGroup)
             .IgnoreAutoIncludes()
             .FirstOrDefaultAsync(u => u.Id.Equals(roleId) && !u.IsDeleted && u.IsActive);
        }



        public async Task<IEnumerable<Permission>> GetAllPermissions()
        {
          return await Context.Permissions.Where(u => !u.IsDeleted && u.IsActive)
         .Include(p => p.PermissionGroup_Permissions)
         .ThenInclude(pgp => pgp.PermissionGroup)
         .ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetPaginatedRolesList(PaginationDto paginationRequest)
        {
            var skipCount = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            IQueryable<Role> query = Context.Roles.Where(u => !u.IsDeleted && u.IsActive);
            if (!string.IsNullOrWhiteSpace(paginationRequest.Searchkey))
            {
                query = query.Where(u => u.RoleName_Farsi.Contains(paginationRequest.Searchkey));
            }
            query = paginationRequest.FilterType == FilterType.Asc ?
                query.OrderBy(u => u.Id) :
                query.OrderByDescending(u => u.Id);
            return await query.Skip(skipCount).Take(paginationRequest.PageSize).ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetMenusByRoleId(List<Guid> menuIds)
        {
           return await Context.Menus.Where(x=>menuIds.Contains(x.Id) && !x.IsDeleted && x.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetMenusList()
        {
            return await Context.Menus.Where(x => !x.IsDeleted && x.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetAllMenus()
        {
            return await Context.Menus
            .Where(u => !u.IsDeleted && u.IsActive)
            .ToListAsync();
        }

        public async Task<User?> GetUserByNationalCode(string nationalCode)
        {
            return await Context.Users.Include(x=>x.Otps).Include(x => x.UserDetail).FirstOrDefaultAsync(x => x.NationalCode.Equals(nationalCode) && !x.IsDeleted && x.IsActive);
        }
        public async Task<User?> GetUserByNoNezam(string noNezam)
        {
            return await Context.Users.Include(x => x.Otps).Include(x => x.UserDetail).FirstOrDefaultAsync(x => x.NoNezam.Equals(noNezam) && !x.IsDeleted && x.IsActive);
        }



        public async Task<Otp?> GetOtpByUserId(Guid userId)
        {
            return await Context.Otps
             .Where(x => x.UserId == userId && !x.IsUsed)
             .OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
        }

      
    }
}
