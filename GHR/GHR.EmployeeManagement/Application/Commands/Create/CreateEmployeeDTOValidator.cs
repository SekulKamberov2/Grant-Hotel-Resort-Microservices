namespace GHR.EmployeeManagement.Application.Commands.Create
{
    using FluentValidation;
    using GHR.EmployeeManagement.Application.DTOs;
    public class CreateEmployeeDTOValidator : AbstractValidator<CreateEmployeeDTO>
    {
        public CreateEmployeeDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50);

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .Must(g => g == "M" || g == "F" || string.IsNullOrEmpty(g))
                .WithMessage("Gender must be 'M' or 'F'.");

            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Hire date cannot be in the future.")
                .When(x => x.HireDate.HasValue);

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department is required.");

            RuleFor(x => x.FacilityId)
                .GreaterThan(0).WithMessage("Facility is required.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email address.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number can't exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Salary must be non-negative.")
                .When(x => x.Salary.HasValue);

            RuleFor(x => x.Status)
                .MaximumLength(20)
                .When(x => !string.IsNullOrEmpty(x.Status));

            RuleFor(x => x.EmergencyContact)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.EmergencyContact));

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    } 
}
