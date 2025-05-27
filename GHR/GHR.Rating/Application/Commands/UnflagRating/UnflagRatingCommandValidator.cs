namespace GHR.Rating.Application.Commands
{
    using FluentValidation;
    public class UnflagRatingCommandValidator : AbstractValidator<UnflagRatingCommand>
    {
        public UnflagRatingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Rating ID must be greater than zero.");
        }
    }
}
