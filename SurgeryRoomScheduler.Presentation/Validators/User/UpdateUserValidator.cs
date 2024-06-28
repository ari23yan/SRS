using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Entities.Account;
using System.Text.RegularExpressions;

namespace SurgeryRoomScheduler.Presentation.Validators.User
{
    public class UpdateUserValidator: AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.RoleId.ToString())
           .Must((model, roleId) => roleId == null || BeAValidGuid(roleId))
           .Must(BeAValidGuid).WithMessage("must be a valid RoleId.");


           RuleFor(x => x.PhoneNumber)
          .Must((model, phoneNumber) => phoneNumber == null || BeAValidPhoneNumber(phoneNumber))
          .WithMessage("Phone number must be a valid phone number.");


        }
        private bool BeAValidGuid(string input)
        {
            if (Guid.TryParse(input, out _))
            {
                return true;
            }
            return false;
        }
        private bool BeAValidPhoneNumber(string input)
        {
            var regex = @"^(0)?9\d{9}$";
            return Regex.IsMatch(input, regex);
        }

    }
}
