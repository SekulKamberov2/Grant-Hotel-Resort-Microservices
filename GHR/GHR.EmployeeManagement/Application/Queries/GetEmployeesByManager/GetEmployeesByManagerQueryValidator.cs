namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByManager
{
    using FluentValidation;
    public class GetEmployeesByManagerQueryValidator : AbstractValidator<GetEmployeesByManagerQuery>
    {
        public GetEmployeesByManagerQueryValidator()
        {
            RuleFor(x => x.ManagerId)
                .GreaterThan(0)
                .WithMessage("ManagerId must be greater than zero.");
        }
    } 
}
