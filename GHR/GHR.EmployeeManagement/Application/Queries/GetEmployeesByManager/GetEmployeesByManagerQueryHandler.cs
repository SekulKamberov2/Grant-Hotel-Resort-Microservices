namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByManager
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel; 
    public class GetEmployeesByManagerQueryHandler : IRequestHandler<GetEmployeesByManagerQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService;
        public GetEmployeesByManagerQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetEmployeesByManagerQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetByManagerAsync(request.ManagerId);
        }
    } 
}
