namespace GHR.EmployeeManagement.Application.Commands.Delete
{
    using FluentValidation;
    public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid employee ID.");
        }
    } 
}
