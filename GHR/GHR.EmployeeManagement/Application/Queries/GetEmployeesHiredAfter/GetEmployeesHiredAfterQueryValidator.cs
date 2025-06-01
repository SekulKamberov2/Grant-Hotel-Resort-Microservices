namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesHiredAfter
{
    using FluentValidation;
    public class GetEmployeesHiredAfterQueryValidator : AbstractValidator<GetEmployeesHiredAfterQuery>
    {
        public GetEmployeesHiredAfterQueryValidator()
        {
            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Date cannot be in the future.");
        }
    } 
}
