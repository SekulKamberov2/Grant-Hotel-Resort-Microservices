namespace GHR.Rating.Application.Commands.ApproveRating
{
    using FluentValidation;
    public class ApproveRatingCommandValidator : AbstractValidator<ApproveRatingCommand>
    {
        public ApproveRatingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than zero.");
        }
    }
}
