namespace GHR.Rating.Application.Commands.AwardTopPerformers
{
    using FluentValidation;
    public class AwardTopPerformersCommandValidator : AbstractValidator<AwardTopPerformersCommand>
    {
        public AwardTopPerformersCommandValidator()
        {
            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Period is required.")
                .Must(BeAValidPeriod).WithMessage("Period must be one of: Weekly, Monthly, Yearly.");
        }

        private bool BeAValidPeriod(string period)
        {
            var validPeriods = new[] { "Weekly", "Monthly", "Yearly" };
            return validPeriods.Contains(period, StringComparer.OrdinalIgnoreCase);
        }
    }
}
