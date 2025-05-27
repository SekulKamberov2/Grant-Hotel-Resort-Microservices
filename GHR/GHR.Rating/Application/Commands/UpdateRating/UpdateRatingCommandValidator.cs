namespace GHR.Rating.Application.Commands.UpdateRating
{
    using FluentValidation;
    public class UpdateRatingCommandValidator : AbstractValidator<UpdateRatingCommand>
    {
        public UpdateRatingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Rating Id must be greater than zero.");

            RuleFor(x => x.Stars)
                .InclusiveBetween(1, 10)
                .WithMessage("Stars must be between 1 and 10.");

            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithMessage("Comment cannot be empty.")
                .MaximumLength(1000)
                .WithMessage("Comment cannot exceed 1000 characters.");
        }
    }
}
