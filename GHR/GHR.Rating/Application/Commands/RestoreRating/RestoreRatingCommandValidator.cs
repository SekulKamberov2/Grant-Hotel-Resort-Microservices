namespace GHR.Rating.Application.Commands.RestoreRating
{
    using FluentValidation; 
    public class RestoreRatingCommandValidator : AbstractValidator<RestoreRatingCommand>
    {
        public RestoreRatingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Rating ID must be greater than zero.");
        }
    }
}
