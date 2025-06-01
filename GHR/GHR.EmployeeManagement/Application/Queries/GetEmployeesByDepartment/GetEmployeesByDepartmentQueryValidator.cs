namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByDepartment
{
    using FluentValidation;
    public class GetEmployeesByDepartmentQueryValidator : AbstractValidator<GetEmployeesByDepartmentQuery>
    {
        public GetEmployeesByDepartmentQueryValidator()
        {
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("DepartmentId must be greater than 0.");
        }
    } 
}
