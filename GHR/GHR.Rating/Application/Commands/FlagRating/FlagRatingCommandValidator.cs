namespace GHR.Rating.Application.Validators
{
    using FluentValidation;
    using GHR.Rating.Application.Commands.FlagRating;
    public class FlagRatingCommandValidator : AbstractValidator<FlagRatingCommand>
    {
        public FlagRatingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Rating ID must be greater than zero.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Flag reason is required.")
                .MaximumLength(500)
                .WithMessage("Flag reason cannot exceed 500 characters.");
        }
    }
}
