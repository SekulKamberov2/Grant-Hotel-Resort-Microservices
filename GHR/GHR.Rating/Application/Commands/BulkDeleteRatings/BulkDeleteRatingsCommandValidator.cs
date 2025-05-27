namespace GHR.Rating.Application.Commands.BulkDeleteRatings
{
    using FluentValidation;
    public class BulkDeleteRatingsCommandValidator : AbstractValidator<BulkDeleteRatingsCommand>
    {
        public BulkDeleteRatingsCommandValidator()
        {
            RuleFor(x => x.RatingIds)
                .NotNull().WithMessage("RatingIds cannot be null.")
                .NotEmpty().WithMessage("At least one rating ID must be provided.");

            RuleForEach(x => x.RatingIds)
                .GreaterThan(0).WithMessage("Rating IDs must be greater than zero.");
        }
    }
}
