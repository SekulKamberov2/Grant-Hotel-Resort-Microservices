namespace GHR.Rating.Application.Queries.GetAwardById
{
    using FluentValidation;
    public class GetAwardByIdQueryValidator : AbstractValidator<GetAwardByIdQuery>
    {
        public GetAwardByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Award ID must be greater than zero.");
        }
    }
}
