namespace GHR.DutyManagement.Application.Commands.AssignDuty
{
    using FluentValidation;
    public class AssignDutyCommandValidator : AbstractValidator<AssignDutyCommand>
    {
        public AssignDutyCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("Employee ID must be greater than zero.");

            RuleFor(x => x.Task)
                .NotEmpty()
                .WithMessage("Task is required.")
                .MaximumLength(200)
                .WithMessage("Task must not exceed 200 characters.");
        }
    }
}
