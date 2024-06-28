using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.User;
using System.Text.RegularExpressions;

namespace SurgeryRoomScheduler.Presentation.Validators.Account
{
    public class LoginUserValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Input).NotNull().NotEmpty()
            .Must(BeAValidUsernameOrMobileOrNationaCode).WithMessage("Input must be a valid username or mobile number Or NationaCode.");
            RuleFor(x => x.Password).NotNull().NotEmpty();

        }

        private bool BeAValidUsernameOrMobileOrNationaCode(string input)
        {
            var usernamePattern = @"^[a-zA-Z0-9]{3,20}$";
            var mobilePattern = @"^\d{11}$";
            string nationalCodepattern = @"/^[0-9]{10}$/";

            return Regex.IsMatch(input, usernamePattern) || Regex.IsMatch(input, mobilePattern) || Regex.IsMatch(input, nationalCodepattern);
        }

    }
}
