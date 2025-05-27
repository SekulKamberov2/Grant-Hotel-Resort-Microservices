namespace GHR.Rating.Application.Commands.UpdateAward
{
    using FluentValidation;
    public class UpdateAwardCommandValidator : AbstractValidator<UpdateAwardCommand>
    {
        public UpdateAwardCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Award ID must be greater than zero.");

            RuleFor(x => x.UsersId)
                .GreaterThan(0)
                .WithMessage("User ID is required and must be greater than zero.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("Department ID is required and must be greater than zero.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(100)
                .WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Period)
                .NotEmpty()
                .WithMessage("Period is required.")
                .Must(p => p == "Weekly" || p == "Monthly" || p == "Yearly")
                .WithMessage("Period must be 'Weekly', 'Monthly', or 'Yearly'.");

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Date cannot be in the future.");
        }
    }
}
