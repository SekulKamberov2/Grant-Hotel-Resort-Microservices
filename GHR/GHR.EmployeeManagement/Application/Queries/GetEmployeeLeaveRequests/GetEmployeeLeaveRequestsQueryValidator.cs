namespace GHR.EmployeeManagement.Application.Queries.GetEmployeeLeaveRequests
{
    using FluentValidation;
    public class GetEmployeeLeaveRequestsQueryValidator : AbstractValidator<GetEmployeeLeaveRequestsQuery>
    {
        public GetEmployeeLeaveRequestsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User Id must be greater than 0.");
        }
    } 
}
