namespace GHR.Rating.Application.Queries.GetAwardByUserId
{
    using FluentValidation;
    public class GetAwardByUserIdQueryValidator : AbstractValidator<GetAwardByUserIdQuery>
    {
        public GetAwardByUserIdQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("Rating ID must be greater than zero.");
        }
    }
}
