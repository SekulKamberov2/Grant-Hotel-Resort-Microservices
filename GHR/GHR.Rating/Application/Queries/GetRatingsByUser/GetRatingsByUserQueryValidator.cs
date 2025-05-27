namespace GHR.Rating.Application.Queries
{
    using FluentValidation;
    public class GetRatingsByUserQueryValidator : AbstractValidator<GetRatingsByUserQuery>
    {
        public GetRatingsByUserQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than zero.");
        }
    }
}
