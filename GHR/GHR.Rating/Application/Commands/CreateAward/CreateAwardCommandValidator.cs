namespace GHR.Rating.Application.Commands.CreateAward
{
    using FluentValidation;
    public class CreateAwardCommandValidator : AbstractValidator<CreateAwardCommand>
    {
        public CreateAwardCommandValidator()
        {
            RuleFor(x => x.UsersId)
                .GreaterThan(0).WithMessage("User Id must be greater than zero.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department Id must be greater than zero.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must be 100 characters or fewer.");

            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Period is required.")
                .Must(BeAValidPeriod).WithMessage("Period must be one of: Weekly, Monthly, Yearly.");

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Award date cannot be in the future.");
        }

        private bool BeAValidPeriod(string period)
        {
            var validPeriods = new[] { "Weekly", "Monthly", "Yearly" };
            return validPeriods.Contains(period, StringComparer.OrdinalIgnoreCase);
        }
    }
}
