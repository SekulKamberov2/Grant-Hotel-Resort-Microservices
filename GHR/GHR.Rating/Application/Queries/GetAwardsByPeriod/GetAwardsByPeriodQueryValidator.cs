namespace GHR.Rating.Application.Queries.GetAwardsByPeriod
{
    using FluentValidation;
    public class GetAwardsByPeriodQueryValidator : AbstractValidator<GetAwardsByPeriodQuery>
    {
        private static readonly string[] ValidPeriods = { "Weekly", "Monthly", "Yearly" }; 
        public GetAwardsByPeriodQueryValidator()
        {
            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Period is required.")
                .Must(p => ValidPeriods.Contains(p))
                .WithMessage($"Period must be one of the following: {string.Join(", ", ValidPeriods)}.");
        }
    }
}
