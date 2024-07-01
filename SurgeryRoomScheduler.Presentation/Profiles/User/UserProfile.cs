using AutoMapper;
using SurgeryRoomScheduler.Application.Security;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos.Permission;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.Common;

namespace SurgeryRoomScheduler.Presentation.Profiles
{
    public class UserProfile : Profile
    {


        public UserProfile()
        {


            CreateMap<Role, GetRolesListDto>().ReverseMap();
            CreateMap<User, UserLoginDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.RoleMenus, opt => opt.Ignore());




            CreateMap<DoctorDto, User>()
           .ForMember(dest => dest.NationalCode, opt => opt.MapFrom(src => src.DoctorNationalCode))
           .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.NoNezam))
           .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => "F054C928-FDC4-469F-8307-4EF1A179F5CE"))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom((src, dest) =>
            {
                string FormatPhoneNumber(string phoneNumber, int type)
                {
                    if (type == 1)
                    {
                        if (!phoneNumber.StartsWith("0"))
                        {
                            return "0" + phoneNumber;
                        }
                        return phoneNumber;
                    }
                    else
                    {
                        if (phoneNumber.StartsWith("0"))
                        {
                            return phoneNumber.Substring(1);
                        }
                        return phoneNumber;
                    }
                }

                if (!string.IsNullOrEmpty(src.Mobile0) && src.Mobile0.Trim().Length > 1)
                {
                    return FormatPhoneNumber(src.Mobile0.Trim(), 1);
                }
                else if (!string.IsNullOrEmpty(src.Mobile1.Trim()) && src.Mobile1.Trim().Length > 1)
                {
                    return FormatPhoneNumber(src.Mobile1, 1);
                }
                else if (!string.IsNullOrEmpty(src.Mobile2.Trim()) && src.Mobile2.Trim().Length > 1)
                {
                    return FormatPhoneNumber(src.Mobile2.Trim(), 1);
                }
                return null;
            }))
           .ForMember(dest => dest.Password, opt => opt.MapFrom((src, dest) =>
           {
               string FormatPhoneNumber(string phoneNumber, int type)
               {
                   if (type == 1)
                   {
                       if (!phoneNumber.StartsWith("0"))
                       {
                           return UtilityManager.EncodePasswordMd5(phoneNumber);
                       }
                       return UtilityManager.EncodePasswordMd5(phoneNumber);
                   }
                   else
                   {
                       if (phoneNumber.StartsWith("0"))
                       {
                           return UtilityManager.EncodePasswordMd5(phoneNumber);
                       }
                       return UtilityManager.EncodePasswordMd5(phoneNumber);
                   }
               }

               if (!string.IsNullOrEmpty(src.Mobile0) && src.Mobile0.Length > 1)
               {
                   return FormatPhoneNumber(src.Mobile0, 2);
               }
               else if (!string.IsNullOrEmpty(src.Mobile1) && src.Mobile1.Length > 1)
               {
                   return FormatPhoneNumber(src.Mobile1, 2);
               }
               else if (!string.IsNullOrEmpty(src.Mobile2) && src.Mobile2.Length > 1)
               {
                   return FormatPhoneNumber(src.Mobile2, 2);
               }
               return null;
           }))
           .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.NP))
           .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.FP))
           .ForMember(dest => dest.NoNezam, opt => opt.MapFrom(src => src.NoNezam))
           .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) =>
           {
               if (src.Active == 1)
               {
                   return true;
               }
               else
               {
                   return false;
               }
           }));



            CreateMap<User, AddUserDto>()
           .ForMember(dest => dest.Gender, opt => opt.Ignore())
           .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore()).ReverseMap();


            CreateMap<UserDetail, AddUserDto>()
            .ForMember(dest => dest.LastName, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.NoNezam, opt => opt.Ignore())
            .ForMember(dest => dest.NationalCode, opt => opt.Ignore())
            .ForMember(dest => dest.FirstName, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore()).ReverseMap();


            CreateMap<Menu, RoleMenusDto>().ReverseMap();
            CreateMap<Role, RoleMenusDto>().ReverseMap();
            CreateMap<Role, GetRoleMenuDto>().ReverseMap();
            CreateMap<Menu, MenusDto>().ReverseMap();



            CreateMap<Role, GetRoleDetailDto>()
           .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());


            CreateMap<UpdateRoleDto, Role>()
                       .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
                       .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                       .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
                       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));




            CreateMap<Role, GetRoleDto>()
           .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleName))
           .ForMember(dest => dest.RoleName_Farsi, opt => opt.MapFrom(src => src.RoleName_Farsi))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
           .ForMember(dest => dest.RolePermissions, opt => opt.MapFrom(src => src.RolePermissions
               .Select(rp => rp.Permission.PermissionGroup_Permissions)
               .SelectMany(pgps => pgps)
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
                       Description = pgp.Permission.Description
                   }).ToList()
               }).ToList()));



            CreateMap<UpdateUserDto, User>()
                       .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
                        .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive != null ? src.IsActive : dest.IsActive))
                        .ForMember(dest => dest.RoleId, opt => opt.MapFrom((src, dest) => src.RoleId != null ? src.RoleId : dest.RoleId))
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())

                       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<UpdateUserDto, UserDetail>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive != null ? src.IsActive : dest.IsActive))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom((src, dest) => src.Gender != null ? src.Gender : dest.Gender))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom((src, dest) => src.DateOfBirth != null ? src.DateOfBirth : dest.DateOfBirth))
            .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<User, UsersListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName_Farsi));




            CreateMap<User, UserDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
            .ForMember(dest => dest.RoleName_Farsi, opt => opt.MapFrom(src => src.Role.RoleName_Farsi))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserDetail.Gender))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.UserDetail.DateOfBirth))
            .ForMember(dest => dest.LastLoginDate, opt => opt.MapFrom(src => src.UserDetail.LastLoginDate))
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.UserDetail.ProfilePicture))
            .ForMember(dest => dest.PermissionGroup, opt => opt.Ignore());

            CreateMap<Permission, PermissionsDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.PermissionName))
                .ForMember(dest => dest.PermissionName_Farsi, opt => opt.MapFrom(src => src.PermissionName_Farsi))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.HasPermission, opt => opt.Ignore());



        }
    }
}
