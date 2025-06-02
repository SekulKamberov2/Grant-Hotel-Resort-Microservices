namespace GHR.EmployeeManagement.Application.Queries.GetEmployeeById
{
    using FluentValidation;
    public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
    {
        public GetEmployeeByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Employee Id must be greater than 0.");
        }
    } 
}
