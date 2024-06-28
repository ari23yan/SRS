using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using System.Text.RegularExpressions;

namespace SurgeryRoomScheduler.Presentation.Validators.Common
{
    public class GetByIdValidator : AbstractValidator<GetByIdDto>
    {
        public GetByIdValidator()
        {
            RuleFor(x => x.TargetId.ToString())
           .NotEmpty().WithMessage("TargetId is required.")
           .Must(BeAValidGuid).WithMessage("must be a valid Id.");



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
