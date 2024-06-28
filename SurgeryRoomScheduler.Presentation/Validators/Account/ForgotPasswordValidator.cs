using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.User;
using System.Text.RegularExpressions;

namespace SurgeryRoomScheduler.Presentation.Validators.Account
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.NationalCode).NotNull().NotEmpty()
            .Must(BeAValidUsernameOrMobileOrNationaCode).WithMessage("Input must be a valid NationaCode.");
        }

        private bool BeAValidUsernameOrMobileOrNationaCode(string input)
        {

            string nationalCodepattern = @"/^[0-9]{10}$/";

            return Regex.IsMatch(input, nationalCodepattern);
        }
    }
}
