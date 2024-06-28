using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.Role;

namespace SurgeryRoomScheduler.Presentation.Validators.Role
{
    public class UpdateRoleValidator: AbstractValidator<UpdateRoleDto>
    {
        public UpdateRoleValidator()
        {
           // RuleFor(x => x.MenuId.ToString())
           // .Must((model, menuId) => menuId == null || BeAValidGuid(menuId))
           //     .Must(BeAValidGuid).WithMessage("must be a valid Menu Id.");

           // RuleFor(x => x.RolePermissions.ToString())
           // .Must((model, RolePermissionId) => RolePermissionId == null || BeAValidGuid(RolePermissionId))
           //.Must(BeAValidGuid).WithMessage("must be a valid RolePermission  Id.");
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
