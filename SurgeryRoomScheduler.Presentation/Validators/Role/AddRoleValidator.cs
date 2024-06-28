using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Entities.Account;

namespace SurgeryRoomScheduler.Presentation.Validators.Role
{
    public class AddRoleValidator: AbstractValidator<AddRoleDto>
    {
        public AddRoleValidator()
        {
            RuleFor(x => x.MenuId.ToString())
            .Must((model, menuId) => menuId == null || BeAValidGuid(menuId))
                .Must(BeAValidGuid).WithMessage("must be a valid Menu Id.");


            RuleFor(x => x.RolePermissions.ToString())
            .Must((model, RolePermissionId) => RolePermissionId == null || BeAValidGuid(RolePermissionId))
           .Must(BeAValidGuid).WithMessage("must be a valid RolePermission  Id.");


            RuleFor(x => x.RoleName)
            .NotNull().WithMessage("Role Name Must Be Fill.");

            RuleFor(x => x.RoleName_Farsi)
             .NotNull().WithMessage("RoleName_Farsi Must Be Fill.");
        }

        private bool BeAValidGuid(string input)
        {
            if (Guid.TryParse(input, out _))
            {
                return true;
            }
            return false;
        }
    }
}
