using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.Sender;
using SurgeryRoomScheduler.Domain.Dtos.User;
using System.Text.RegularExpressions;

namespace SurgeryRoomScheduler.Presentation.Validators.Account
{
    public class ConfirmOtpValidator : AbstractValidator<ConfirmOtpDto>
    {
        public ConfirmOtpValidator()
        {
            RuleFor(x => x.PhoneNumber).NotNull().NotEmpty()
           .Must(BeAValidMobileNum).WithMessage("Input must be a valid username or mobile number Or NationaCode.");
            RuleFor(x => x.Otp).NotNull().NotEmpty();
        }

        private bool BeAValidMobileNum(string input)
        {
            var mobilePattern = @"^\d{11}$";

            return  Regex.IsMatch(input, mobilePattern) ;
        }
    }
}
