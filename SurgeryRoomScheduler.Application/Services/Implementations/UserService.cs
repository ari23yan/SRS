using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using SurgeryRoomScheduler.Application.Security;
using SurgeryRoomScheduler.Application.Senders;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Domain.Dtos.Common.AccessLog;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Permission;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.Sender;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Domain.Interfaces;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Otp> _otpRepository;
        private readonly IRepository<RolePermission> _rolePermissionRepository;
        private readonly IRepository<RoleMenu> _roleMenuRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly ILogService _logService;

        public UserService(IUserRepository userRepository, ILogService logService,
            IRepository<Role> roleRepository, IRepository<RolePermission> rolePermissionRepository,
            IRepository<RoleMenu> roleMenuRepository, IRepository<Otp> otpRepository, ISender sender
            , IPasswordHasher passwordHasher, IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _roleMenuRepository = roleMenuRepository;
            _otpRepository = otpRepository;
            _sender = sender;
            _logService = logService;   
        }
        public async Task<bool> CheckUserHavePermission(Guid roleId, Guid permissionId)
        {
            return await _userRepository.CheckUserHavePermission(roleId, permissionId);
        }

        public async Task<ResponseDto<bool>> DeleteUserByUserId(Guid userId, Guid operatorId)
        {
            var user = await _userRepository.GetUserByUserId(userId);
            if (user == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "Failed" };
            }
            user.IsDeleted = true;
            user.DeletedDate = DateTime.UtcNow;
            user.DeletedBy = operatorId;
            await _userRepository.UpdateAsync(user);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "SuccessFull" };
        }

        public async Task<ResponseDto<IEnumerable<GetRolesListDto>>> GetPaginatedRolesList(PaginationDto request)
        {
            var roles = await _userRepository.GetPaginatedRolesList(request);
            var mappedRoles = _mapper.Map<IEnumerable<Role>, IEnumerable<GetRolesListDto>>(roles.List);
            return new ResponseDto<IEnumerable<GetRolesListDto>>
            {
                IsSuccessFull = true,
                Data = mappedRoles,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = roles.TotalCount,
            };
        }

        public async Task<ResponseDto<IEnumerable<UsersListDto>>> GetPaginatedUsersList(PaginationDto request)
        {
            var users = await _userRepository.GetPaginatedUsersList(request);
            var mappedUsers = _mapper.Map<IEnumerable<User>, IEnumerable<UsersListDto>>(users.List);
            return new ResponseDto<IEnumerable<UsersListDto>>
            {
                IsSuccessFull = true,
                Data = mappedUsers,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = users.TotalCount
            };

        }

        public async Task<ResponseDto<IEnumerable<PermissionGroupDto>>> GetRole(Guid roleId)
        {
            var role = await _userRepository.GetRoleById(roleId);
            if (role == null)
            {
                return new ResponseDto<IEnumerable<PermissionGroupDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "NotFound" };
            }
            var allPermissions = await _userRepository.GetAllPermissions();
            var userRolePermissions = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
            var allPermissionGroups = allPermissions
                .SelectMany(p => p.PermissionGroup_Permissions)
                .GroupBy(pgp => pgp.PermissionGroup)
                .Select(g => new PermissionGroupDto
                {
                    Id = g.Key.Id,
                    PermissionGroupName = g.Key.PermissionGroupName,
                    PermissionGroupName_Farsi = g.Key.PermissionGroupName_Farsi,
                    Description = g.Key.Description,
                    Permissions = g.Select(pgp => new PermissionsDto
                    {
                        Id = pgp.Permission.Id,
                        PermissionName = pgp.Permission.PermissionName,
                        PermissionName_Farsi = pgp.Permission.PermissionName_Farsi,
                        Description = pgp.Permission.Description,
                        HasPermission = userRolePermissions.Contains(pgp.Permission.Id)
                    }).ToList()
                }).ToList();
            return new ResponseDto<IEnumerable<PermissionGroupDto>> { IsSuccessFull = true, Data = allPermissionGroups, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<IEnumerable<GetRolesListDto>>> GetRolesList()
        {
            var roleList = await _userRepository.GetRolesList();
            var mappedRoleList = _mapper.Map<IEnumerable<GetRolesListDto>>(roleList);
            return new ResponseDto<IEnumerable<GetRolesListDto>> { IsSuccessFull = true, Data = mappedRoleList, Message = ErrorsMessages.Success, Status = "Successful" };
        }
        public async Task<ResponseDto<UserDetailDto>> GetUserDetailByUserId(Guid userId)
        {
            var user = await _userRepository.GetUserDetailById(userId);
            if (user == null)
            {
                return new ResponseDto<UserDetailDto> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "NotFound" };
            }
            var allPermissions = await _userRepository.GetAllPermissions();
            var userDetailDto = _mapper.Map<UserDetailDto>(user);
            var userRolePermissions = user.Role.RolePermissions.Select(rp => rp.PermissionId).ToList();
            var allPermissionGroups = allPermissions
                .SelectMany(p => p.PermissionGroup_Permissions)
                .GroupBy(pgp => pgp.PermissionGroup)
                .Select(g => new PermissionGroupDto
                {
                    Id = g.Key.Id,
                    PermissionGroupName = g.Key.PermissionGroupName,
                    PermissionGroupName_Farsi = g.Key.PermissionGroupName_Farsi,
                    Description = g.Key.Description,
                    Permissions = g.Select(pgp => new PermissionsDto
                    {
                        Id = pgp.Permission.Id,
                        PermissionName = pgp.Permission.PermissionName,
                        PermissionName_Farsi = pgp.Permission.PermissionName_Farsi,
                        Description = pgp.Permission.Description,
                        HasPermission = userRolePermissions.Contains(pgp.Permission.Id)
                    }).ToList()
                }).ToList();
            userDetailDto.PermissionGroup = allPermissionGroups;
            return new ResponseDto<UserDetailDto> { IsSuccessFull = true, Data = userDetailDto, Message = ErrorsMessages.Success, Status = "SuccessFull" };
        }
        public async Task<int> GetUsersCount()
        {
            return await _userRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted);
        }
        public async Task<int> GetRolesCount()
        {
            return await _roleRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted);
        }
        public async Task<(UserAuthResponse Result, User? user)> LoginUser(LoginUserDto request)
        {
            User user = new User();
            if (UtilityManager.IsMobile(request.Input))
            {
                user = await _userRepository.GetUserByMobile(request.Input);
            }
            else if (UtilityManager.IsValidNationalCode(request.Input))
            {
                user = await _userRepository.GetUserByNationalCode(request.Input);
            }
            else
            {
                user = await _userRepository.GetUserByUsername(request.Input);
            }

            if (user == null)
            {
                return (UserAuthResponse.NotFound, null);
            }
            if (!user.IsActive)
            {
                return (UserAuthResponse.NotAvtive, user);
            }
            if (user.IsDeleted)
            {
                return (UserAuthResponse.IsDeleted, null);
            }
            var password = _passwordHasher.EncodePasswordMd5(request.Password);
            if (user.Password != password)
            {
                return (UserAuthResponse.WrongPassword, null);
            }
            if (user.RoleId.ToString() != "f2a3bbee-ee97-4cb6-ab48-fbce464c0c56") //Developer
            {
                var thirtyMinutesAgo = DateTime.Now.AddMinutes(-20);

                var userTryToLoginCount = await _otpRepository.GetCountAsync(x =>
                    x.UserId.Equals(user.Id) &&
                    !x.IsUsed &&
                    x.CreatedDate >= thirtyMinutesAgo &&
                    x.CreatedDate <= DateTime.Now
                );

                bool hasExceededAttempts = userTryToLoginCount >= 5;


                if (hasExceededAttempts)
                {
                    user.IsActive = false;
                    await _userRepository.UpdateAsync(user);
                    return (UserAuthResponse.TooManyTries, null);
                }
                user.LastLoginDate = DateTime.Now;
                await _userRepository.UpdateAsync(user);
            }

            return (UserAuthResponse.Success, user);
        }
        public async Task<ResponseDto<AddUserDto>> AddUser(AddUserDto request, Guid operatorId)
        {
            if (await _userRepository.CheckUserExistByPhoneMumber(request.PhoneNumber))
            {
                return new ResponseDto<AddUserDto> { IsSuccessFull = false, Message = ErrorsMessages.PhoneNumberAlreadyExist, Status = "phone number already exist" };
            }
            if (await _userRepository.CheckUserExistByUsername(request.Username))
            {
                return new ResponseDto<AddUserDto> { IsSuccessFull = false, Message = ErrorsMessages.UsernameAlreadyExist, Status = "username number already exist" };
            }
            request.CreatedBy = operatorId;
            var mappedUser = _mapper.Map<User>(request);
            await _userRepository.AddAsync(mappedUser);
            return new ResponseDto<AddUserDto> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<bool>> UpdateUserByUserId(Guid userId, UpdateUserDto request, Guid operatorId)
        {
            var user = await _userRepository.GetUserWithDetailsById(userId);
            if (user == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "User Not Found" };
            }
            if (request.PhoneNumber != null && request.PhoneNumber != "" && user.PhoneNumber != request.PhoneNumber)
            {
                if (await _userRepository.CheckUserExistByPhoneMumber(request.PhoneNumber))
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.PhoneNumberAlreadyExist, Status = "phone number already exist" };
                }
            }
            if (request.Username != null && request.Username != "" && user.Username != request.Username)
            {
                if (await _userRepository.CheckUserExistByUsername(request.Username))
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.UsernameAlreadyExist, Status = "username number already exist" };
                }
            }
            if (request.RoleId != null && request.RoleId != user.RoleId)
            {
                var logDto = new InsertUserAccessLogDto
                {
                    UserId = userId,
                    OperatorId = operatorId,
                };
                await _logService.InserAccessLog(AccessLogType.RoleChange, logDto);
            }
            var mappedUser = _mapper.Map(request, user);
            mappedUser.ModifiedBy = operatorId;
            mappedUser.IsModified = true;
            await _userRepository.UpdateAsync(mappedUser);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };

        }

        public async Task<ResponseDto<GetRoleDetailDto>> GetRoleDetailByRoleId(Guid roleId)
        {
            var role = await _userRepository.GetRoleWithDetailById(roleId);
            if (role == null)
            {
                return new ResponseDto<GetRoleDetailDto> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "NotFound" };
            }
            var allPermissions = await _userRepository.GetAllPermissions();
            var allMenus = await _userRepository.GetMenusList();
            var userRolemenuIds = role.RoleMenus.Select(x => x.MenuId).ToList();
            var roleMenu = await GetMenus(userRolemenuIds, allMenus);

            var roleDetailDto = _mapper.Map<GetRoleDetailDto>(role);
            roleDetailDto.Menus = roleMenu;
            var roleRolePermissions = role.RolePermissions.Select(rp => rp.PermissionId).ToList();

            var allPermissionGroups = allPermissions
                .SelectMany(p => p.PermissionGroup_Permissions)
                .GroupBy(pgp => pgp.PermissionGroup)
                .Select(g => new PermissionGroupDto
                {
                    Id = g.Key.Id,
                    PermissionGroupName = g.Key.PermissionGroupName,
                    PermissionGroupName_Farsi = g.Key.PermissionGroupName_Farsi,
                    Description = g.Key.Description,
                    Permissions = g.Select(pgp => new PermissionsDto
                    {
                        Id = pgp.Permission.Id,
                        PermissionName = pgp.Permission.PermissionName,
                        PermissionName_Farsi = pgp.Permission.PermissionName_Farsi,
                        Description = pgp.Permission.Description,
                        HasPermission = roleRolePermissions.Contains(pgp.Permission.Id)
                    }).ToList()
                }).ToList();
            roleDetailDto.RolePermissions = allPermissionGroups;
            return new ResponseDto<GetRoleDetailDto> { IsSuccessFull = true, Data = roleDetailDto, Message = ErrorsMessages.Success, Status = "SuccessFull" };
        }

        public async Task<ResponseDto<GetRoleMenuDto>> GetRoleMenusByRoleId(Guid roleId)
        {
            var role = await _userRepository.GetRoleWithDetailById(roleId);
            if (role == null)
            {
                return new ResponseDto<GetRoleMenuDto> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "NotFound" };
            }
            var allPermissions = await _userRepository.GetAllPermissions();
            var allMenus = await _userRepository.GetMenusList();
            var userRolemenuIds = role.RoleMenus.Select(x => x.MenuId).ToList();
            var roleMenu = await GetMenus(userRolemenuIds, allMenus);
            var roleDetailDto = _mapper.Map<GetRoleMenuDto>(role);
            roleDetailDto.Menus = roleMenu;
            return new ResponseDto<GetRoleMenuDto> { IsSuccessFull = true, Data = roleDetailDto, Message = ErrorsMessages.Success, Status = "SuccessFull" };
        }


        public async Task<List<RoleMenusDto>> GetMenus(List<Guid> menuIds, IEnumerable<Menu> allMenus)
        {
            var mappedAllMenus = _mapper.Map<List<RoleMenusDto>>(allMenus);
            var res = await BuildMenuHierarchy(mappedAllMenus);
            foreach (var item in res)
            {
                if (item.SubMenus != null && item.SubMenus.Count() > 0)
                {
                    foreach (var subItem in item.SubMenus)
                    {
                        if (menuIds.Contains(subItem.Id))
                        {
                            subItem.HasMenu = true;
                        }
                    }
                }
                if (menuIds.Contains(item.Id))
                {
                    item.HasMenu = true;
                }

            }
            return res;
        }

        public async Task<List<RoleMenusDto>> BuildMenuHierarchy(IEnumerable<RoleMenusDto> allMenus)
        {
            var menuDict = allMenus.ToDictionary(menu => menu.Id, menu => menu);
            var rootMenus = new List<RoleMenusDto>();

            foreach (var menu in allMenus)
            {
                if (menuDict.TryGetValue(menu.Id, out var menuNode))
                {
                    if (menu.SubMenuId.HasValue && menuDict.TryGetValue(menu.SubMenuId.Value, out var parentMenuNode))
                    {
                        if (parentMenuNode.SubMenus == null)
                        {
                            parentMenuNode.SubMenus = new List<RoleMenusDto>();
                        }
                        parentMenuNode.SubMenus.Add(menuNode);
                    }
                    else
                    {
                        rootMenus.Add(menuNode);
                    }
                }
            }
            return rootMenus;
        }

        public async Task<ResponseDto<bool>> DeleteRoleByRoleId(Guid roleId, Guid operatorId)
        {
            var role = await _userRepository.GetRoleById(roleId);
            if (role == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "Failed" };
            }
            role.IsDeleted = true;
            role.DeletedDate = DateTime.UtcNow;
            role.DeletedBy = operatorId;
            await _roleRepository.UpdateAsync(role);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "SuccessFull" };
        }

        public async Task<ResponseDto<bool>> UpdateRoleByRoleId(Guid roleId, UpdateRoleDto request, Guid operatorId)
        {
            var role = await _userRepository.GetRoleWithDetailById(roleId);
            if (role == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "User Not Found" };
            }

            var mappedRole = _mapper.Map(request, role);

            if (request.MenuId != null && request.MenuId.Count() > 0) // Role menus
            {
                mappedRole.RoleMenus.Clear();
                foreach (var menu in request.MenuId)
                {
                    role.RoleMenus.Add(new RoleMenu
                    {
                        MenuId = menu,
                        RoleId = role.Id,
                        CreatedBy = operatorId,
                    });
                }
                var logDto = new InsertUserAccessLogDto
                {
                    UserId = roleId,
                    OperatorId = operatorId
                };
                await _logService.InserAccessLog(AccessLogType.MenuChange, logDto);
            }


            if (request.RolePermissions != null && request.RolePermissions.Count() > 0)// Role Permissions
            {
                mappedRole.RolePermissions.Clear();
                foreach (var permission in request.RolePermissions)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        PermissionId = permission,
                        RoleId = role.Id,
                        CreatedBy = operatorId,
                    });
                }
                var logDto = new InsertUserAccessLogDto
                {
                    UserId = roleId,
                    OperatorId = operatorId
                };
                await _logService.InserAccessLog(AccessLogType.PermissionChange, logDto);
            }
            role.ModifiedBy = operatorId;
            role.IsModified = true;
            await _roleRepository.UpdateAsync(role);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }



        public async Task<ResponseDto<bool>> CreateRole(AddRoleDto request, Guid operatorId)
        {
            var check = await _roleRepository.IsExist(x => x.RoleName == request.RoleName);
            if (check)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Exist, Status = "Failed" };
            }
            var newRole = new Role
            {
                RoleName = request.RoleName,
                RoleName_Farsi = request.RoleName_Farsi,
                Description = request.Description,
                CreatedBy = operatorId
            };
            await _roleRepository.AddAsync(newRole);

            List<RoleMenu> RoleMenuList = new List<RoleMenu>(); /// Role Menus
            foreach (var item in request.MenuId)
            {
                RoleMenu roleMenu = new RoleMenu
                {
                    RoleId = newRole.Id,
                    MenuId = item,
                    CreatedBy = operatorId,
                };
                RoleMenuList.Add(roleMenu);
            }
            await _roleMenuRepository.AddRangeAsync(RoleMenuList);


            List<RolePermission> RolePermissionList = new List<RolePermission>();/// Role Permissions
            foreach (var item in request.RolePermissions)
            {
                RolePermission rolePermission = new RolePermission
                {
                    RoleId = newRole.Id,
                    PermissionId = item,
                    CreatedBy = operatorId,
                };
                RolePermissionList.Add(rolePermission);
            }
            await _rolePermissionRepository.AddRangeAsync(RolePermissionList);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<IEnumerable<PermissionGroupDto>>> GetPermissionList()
        {
            var allPermissions = await _userRepository.GetAllPermissions();
            var allPermissionGroups = allPermissions
                .SelectMany(p => p.PermissionGroup_Permissions)
                .GroupBy(pgp => pgp.PermissionGroup)
                .Select(g => new PermissionGroupDto
                {
                    Id = g.Key.Id,
                    PermissionGroupName = g.Key.PermissionGroupName,
                    PermissionGroupName_Farsi = g.Key.PermissionGroupName_Farsi,
                    Description = g.Key.Description,
                    Permissions = g.Select(pgp => new PermissionsDto
                    {
                        Id = pgp.Permission.Id,
                        PermissionName = pgp.Permission.PermissionName,
                        PermissionName_Farsi = pgp.Permission.PermissionName_Farsi,
                        Description = pgp.Permission.Description,
                        HasPermission = false
                    }).ToList()
                }).ToList();
            return new ResponseDto<IEnumerable<PermissionGroupDto>> { IsSuccessFull = true, Data = allPermissionGroups, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<IEnumerable<MenusDto>>> GetMenusList()
        {
            var menus = await _userRepository.GetMenusList();
            var mappedMenus = _mapper.Map<IEnumerable<MenusDto>>(menus);
            return new ResponseDto<IEnumerable<MenusDto>> { IsSuccessFull = true, Data = mappedMenus, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<bool> InsertOtp(string otpCode, Guid userId)
        {
            var newOtp = new Otp
            {
                OtpCode = otpCode,
                UserId = userId,
            };
            await _otpRepository.AddAsync(newOtp);
            return true;
        }

        public async Task<ResponseDto<UserLoginDto>> ConfirmOtp(ConfirmOtpDto request)
        {
            var user = await _userRepository.GetUserAndOtpByMobile(request.PhoneNumber);
            if (user == null)
            {
                return new ResponseDto<UserLoginDto> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "لطفا مجدد درخواست دهید" };
            }
            var getUserOtp = await _userRepository.GetOtpByUserId(user.Id);
            if (getUserOtp == null)
            {
                return new ResponseDto<UserLoginDto> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "لطفا مجدد درخواست دهید" };
            }
            if (DateTime.Now >= getUserOtp.ExpirationTime)
            {
                return new ResponseDto<UserLoginDto> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کد وارد شده منقضی شده است" };
            }
            if (getUserOtp.OtpCode != request.Otp)
            {
                return new ResponseDto<UserLoginDto> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کد وارد شده صحیح نمی باشد" };
            }
            getUserOtp.IsUsed = true;
            await _otpRepository.UpdateAsync(getUserOtp);
            var mappedUser = _mapper.Map<UserLoginDto>(user);
            var Menu = await GetRoleMenusByRoleId(user.RoleId);
            mappedUser.RoleMenus = Menu.Data;


            return new ResponseDto<UserLoginDto> { IsSuccessFull = true, Data = mappedUser, Message = ErrorsMessages.Success, Status = "ورود موفقیت آمیز" };
        }

        public async Task<ResponseDto<bool>> ForgotPassword(ForgotPasswordDto request)
        {
            User user = new User();
            if (UtilityManager.IsValidNationalCode(request.NationalCode))
            {
                user = await _userRepository.GetUserByNationalCode(request.NationalCode);
            }
            else
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کد ملی وارد شده اشتباه می باشد." };
            }
            if (user == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کاربر با مشخصات ارسالی یافت نشد" };
            }
            if (!user.IsActive)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کاربر غیرفعال می باشد" };
            }
            if (user.IsDeleted)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "یافت نشد" };
            }
            var userOtp = UtilityManager.OtpGenrator();
            if (user.RoleId.ToString() != "f2a3bbee-ee97-4cb6-ab48-fbce464c0c56") //Developer
            {
                var loginSms = await _sender.LoginUserSendSmsAsync(user.PhoneNumber, userOtp);
                if (!loginSms)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "اشکال در ارسال پیامک" };
                }
                await InsertOtp(userOtp, user.Id);
            }
            else
                await InsertOtp("654321", user.Id);

            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = $"کد بازیابی رمز عبور برای شماره تلفن همراه {user.PhoneNumber} شما ارسال شد " };
        }

        public async Task<ResponseDto<bool>> SubmitPassword(SubmitPasswordDto request)
        {
            var user = await _userRepository.GetUserByMobile(request.PhoneNumber);
            if (user == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "یافت نشد" };
            }
            var getUserOtp = user.Otps
            .Where(x => x.UserId == user.Id && !x.IsUsed)
            .OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (getUserOtp == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "لطفا مجدد درخواست دهید" };
            }
            if (DateTime.Now >= getUserOtp.ExpirationTime)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کد وارد شده منقضی شده است" };
            }
            if (getUserOtp.OtpCode != request.Otp)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کد وارد شده صحیح نمی باشد" };
            }
            var hashedPassword = _passwordHasher.EncodePasswordMd5(request.Password);
            user.Password = hashedPassword;
            await _userRepository.UpdateAsync(user);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "عملیات موفقیت آمیز" };

        }

        public async Task<bool> DeleteDoctors()
        {
            return await _userRepository.DeleteDoctors();
        }
    }
}
