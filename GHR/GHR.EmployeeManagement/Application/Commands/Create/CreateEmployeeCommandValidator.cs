namespace GHR.EmployeeManagement.Application.Commands.Create
{
    using FluentValidation;
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.Employee).NotNull().WithMessage("Employee is required.")
                .SetValidator(new CreateEmployeeDTOValidator());
        }
    } 
}
