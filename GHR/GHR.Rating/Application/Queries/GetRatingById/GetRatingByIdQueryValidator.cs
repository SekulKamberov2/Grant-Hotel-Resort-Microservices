namespace GHR.Rating.Application.Queries.GetRatingById
{
    using FluentValidation;
    public class GetRatingByIdQueryValidator : AbstractValidator<GetRatingByIdQuery>
    {
        public GetRatingByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Rating ID must be greater than zero.");
        }
    }
}
