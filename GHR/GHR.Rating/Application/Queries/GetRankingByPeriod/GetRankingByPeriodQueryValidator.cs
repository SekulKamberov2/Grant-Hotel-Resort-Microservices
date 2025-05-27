namespace GHR.Rating.Application.Queries.GetRankingByPeriod
{
    using FluentValidation;
    public class GetRankingByPeriodQueryValidator : AbstractValidator<GetRankingByPeriodQuery>
    {
        public GetRankingByPeriodQueryValidator()
        {
            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Period is required.")
                .Matches(@"^\d{4}-(0[1-9]|1[0-2])$").WithMessage("Period must be in the format 'YYYY-MM'.");
        }
    }
}
