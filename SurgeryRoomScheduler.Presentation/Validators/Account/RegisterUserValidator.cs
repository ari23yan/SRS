using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Enums;

namespace SurgeryRoomScheduler.Presentation.Validators.Account
{
    public class UserValidator : AbstractValidator<RegisterUserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName).NotNull().NotEmpty();
            RuleFor(x => x.LastName).NotNull().NotEmpty();
            RuleFor(x => x.PhoneNumber).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty().Equal(x => x.ConfirmPassword).WithMessage("Password And Confirm Password Is Not Match !");
        }
    }
}
