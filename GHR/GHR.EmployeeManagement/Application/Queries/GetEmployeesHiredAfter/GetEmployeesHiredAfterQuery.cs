namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesHiredAfter
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class GetEmployeesHiredAfterQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
        public DateTime Date { get; }
        public GetEmployeesHiredAfterQuery(DateTime date) => Date = date;
    }
}
