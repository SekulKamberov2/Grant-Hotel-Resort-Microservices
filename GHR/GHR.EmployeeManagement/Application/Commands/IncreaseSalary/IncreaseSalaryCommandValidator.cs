namespace GHR.EmployeeManagement.Application.Commands.IncreaseSalary
{
    using FluentValidation;
    public class IncreaseSalaryCommandValidator : AbstractValidator<IncreaseSalaryCommand>
    {
        public IncreaseSalaryCommandValidator()
        {
            RuleFor(x => x.Years)
                .GreaterThan(0).WithMessage("Years must be greater than 0.");

            RuleFor(x => x.Percentage)
                .GreaterThan(0).WithMessage("Percentage must be greater than 0.");
        }
    } 
}
