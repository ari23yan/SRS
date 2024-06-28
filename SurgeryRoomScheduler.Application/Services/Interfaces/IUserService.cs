using SurgeryRoomScheduler.Domain.Dtos.Common.AccessLog;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Permission;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.Sender;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDto<AddUserDto>> AddUser(AddUserDto requset, Guid operatorId);
        Task<(UserAuthResponse Result, User? user)> LoginUser(LoginUserDto input);
        Task<ResponseDto<UserLoginDto>> ConfirmOtp(ConfirmOtpDto request);
        Task<ResponseDto<bool>> ForgotPassword(ForgotPasswordDto request);
        Task<ResponseDto<bool>> SubmitPassword(SubmitPasswordDto request);
        Task<bool> InsertOtp(string otpCode,Guid userId);
        Task<bool> CheckUserHavePermission(Guid roleId, Guid permissionId);
        Task<ResponseDto<UserDetailDto>> GetUserDetailByUserId(Guid userId);
        Task<ResponseDto<GetRoleDetailDto>> GetRoleDetailByRoleId(Guid roleId);
        Task<ResponseDto<GetRoleMenuDto>> GetRoleMenusByRoleId(Guid roleId);
        Task<ResponseDto<bool>> DeleteUserByUserId(Guid userId,Guid operatorId);
        Task<ResponseDto<bool>> DeleteRoleByRoleId(Guid roleId,Guid operatorId);
        Task<ResponseDto<bool>> UpdateUserByUserId(Guid userId,UpdateUserDto request, Guid operatorId);
        Task<ResponseDto<bool>> UpdateRoleByRoleId(Guid roleId,UpdateRoleDto request, Guid operatorId);
        Task<ResponseDto<IEnumerable<UsersListDto>>> GetPaginatedUsersList(PaginationDto request);
        Task<ResponseDto<IEnumerable<GetRolesListDto>>> GetPaginatedRolesList(PaginationDto request);
        Task<ResponseDto<IEnumerable<GetRolesListDto>>> GetRolesList();
        Task<ResponseDto<IEnumerable<PermissionGroupDto>>> GetPermissionList();
        Task<ResponseDto<IEnumerable<MenusDto>>> GetMenusList();
        Task<ResponseDto<IEnumerable<PermissionGroupDto>>> GetRole(Guid roleId);
        Task<ResponseDto<bool>> CreateRole(AddRoleDto request, Guid operatorId);
        Task<int> GetUsersCount();
        Task<int> GetRolesCount();
        Task<bool> InserAccessLog(AccessLogType type,InsertUserAccessLogDto request);
    }
}
