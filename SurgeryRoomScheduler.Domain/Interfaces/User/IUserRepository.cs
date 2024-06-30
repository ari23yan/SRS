using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Interfaces.Users
{
    public interface IUserRepository: IRepository<User>
    {
        Task<bool> CheckUserExist(string loginIdentifier);
        Task<bool> CheckUserExistByPhoneMumber(string phoneNumber);
        Task<bool> CheckUserExistByUsername(string username);
        Task<User?> GetUserByMobile(string mobile);
        Task<User?> GetUserAndOtpByMobile(string mobile);
        Task<Otp?> GetOtpByUserId(Guid userId);
        Task<User?> GetUserByUserId(Guid userId);
        Task<User?> GetUserWithDetailsById(Guid userId);
        Task<User?> GetUserWithRolesByUserId(Guid userId);
        Task<bool> CheckUserHavePermission(Guid roleId, Guid permissionId);
        Task<User?> GetUserByUsername(string userName);
        Task<User?> GetUserByNoNezam(string noNezam);
        Task<User?> GetUserByNationalCode(string nationalCode);
        Task<User?> GetUserDetailById(Guid userId);
        Task<IEnumerable<Permission>> GetAllPermissions();
        Task<IEnumerable<Menu>> GetAllMenus();
        Task<IEnumerable<User>> GetPaginatedUsersList(PaginationDto request);
        Task<IEnumerable<Role>> GetPaginatedRolesList(PaginationDto request);
        Task<IEnumerable<Role>> GetRolesList();
        Task<IEnumerable<Menu>> GetMenusList();
        Task<Role> GetRoleById(Guid roleId);
        Task<IEnumerable<Menu>> GetMenusByRoleId(List<Guid>  menuIds);
        Task<Role> GetRoleWithDetailById(Guid roleId);
    }
}
