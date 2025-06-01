namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesSalaryAbove
{
    using FluentValidation;
    public class GetEmployeesSalaryAboveQueryValidator : AbstractValidator<GetEmployeesSalaryAboveQuery>
    {
        public GetEmployeesSalaryAboveQueryValidator()
        {
            RuleFor(x => x.Salary)
                .GreaterThan(0)
                .WithMessage("Salary must be greater than zero.");
        }
    } 
}
