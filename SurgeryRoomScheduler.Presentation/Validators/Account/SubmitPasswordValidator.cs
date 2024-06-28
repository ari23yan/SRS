using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.Sender;
using SurgeryRoomScheduler.Domain.Dtos.User;
using System.Text.RegularExpressions;

namespace SurgeryRoomScheduler.Presentation.Validators.Account
{
    public class SubmitPasswordValidator : AbstractValidator<SubmitPasswordDto>
    {
        public SubmitPasswordValidator()
        {
            RuleFor(x => x.PhoneNumber).NotNull().NotEmpty()
           .Must(BeAValidMobileNum).WithMessage("Input must be a valid username or mobile number Or NationaCode.");
            RuleFor(x => x.Otp).NotNull().NotEmpty();

               RuleFor(x => x.Password)
            .NotNull().WithMessage("Password cannot be null.")
            .NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Equal(x => x.ConfirmPassword).WithMessage("Password and Confirm Password must match.");

            RuleFor(x => x.ConfirmPassword)
            .NotNull().WithMessage("Confirm Password cannot be null.")
            .NotEmpty().WithMessage("Confirm Password cannot be empty.");
        }

        private bool BeAValidMobileNum(string input)
        {
            var mobilePattern = @"^\d{11}$";

            return Regex.IsMatch(input, mobilePattern);
        }
    }
}
