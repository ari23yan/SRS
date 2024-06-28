using FluentValidation;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.User;

namespace SurgeryRoomScheduler.Presentation.Validators.Common
{
    public class PaginationValidator: AbstractValidator<PaginationDto>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).NotEmpty().NotEmpty();
            RuleFor(x => x.PageSize).LessThanOrEqualTo(50).NotNull().NotEmpty();
        }
    }
}
