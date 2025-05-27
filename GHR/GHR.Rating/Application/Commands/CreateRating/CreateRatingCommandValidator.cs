namespace GHR.Rating.Application.Commands.CreateRating
{
    using FluentValidation;
    public class CreateRatingCommandValidator : AbstractValidator<CreateRatingCommand>
    {
        public CreateRatingCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than zero.");

            RuleFor(x => x.ServiceId)
                .InclusiveBetween(1, 3).WithMessage("ServiceId must be between 1 and 3 "); //Fitness, HR, HelpDesk

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("DepartmentId must be greater than zero.");

            RuleFor(x => x.Stars)
                .InclusiveBetween(1, 10).WithMessage("Stars must be between 1 and 10.");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
        }
    }
}
