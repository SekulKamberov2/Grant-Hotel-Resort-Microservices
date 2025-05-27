namespace GHR.Rating.Application.Queries.Validators
{
    using FluentValidation;
    public class GetRatingsByServiceQueryValidator : AbstractValidator<GetRatingsByServiceQuery>
    {
        public GetRatingsByServiceQueryValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage("ServiceId must be greater than zero.");
        }
    }
}
