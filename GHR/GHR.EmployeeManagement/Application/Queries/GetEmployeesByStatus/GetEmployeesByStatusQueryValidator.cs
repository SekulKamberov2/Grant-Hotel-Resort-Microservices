namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByStatus
{
    using FluentValidation;
    public class GetEmployeesByStatusQueryValidator : AbstractValidator<GetEmployeesByStatusQuery>
    {
        public GetEmployeesByStatusQueryValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .MaximumLength(50)
                .WithMessage("Status must be 50 characters or less.");
        }
    } 
}
