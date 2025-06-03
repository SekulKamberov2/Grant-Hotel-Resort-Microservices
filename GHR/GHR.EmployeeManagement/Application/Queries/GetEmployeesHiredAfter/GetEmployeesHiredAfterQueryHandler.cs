namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesHiredAfter
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;

    public class GetEmployeesHiredAfterQueryHandler : IRequestHandler<GetEmployeesHiredAfterQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService;
        public GetEmployeesHiredAfterQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetEmployeesHiredAfterQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetHiredAfterAsync(request.Date);
        }
    } 
}
