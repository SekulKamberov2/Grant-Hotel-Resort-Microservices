namespace GHR.EmployeeManagement.Application.Commands.Update
{
    using FluentValidation;
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid employee ID.");

            RuleFor(x => x.Employee.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.Employee.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Employee.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.Employee.DepartmentId).GreaterThan(0);
            RuleFor(x => x.Employee.FacilityId).GreaterThan(0);
            RuleFor(x => x.Employee.Salary).GreaterThan(0).WithMessage("Salary must be greater than zero."); 
        }
    } 
}
