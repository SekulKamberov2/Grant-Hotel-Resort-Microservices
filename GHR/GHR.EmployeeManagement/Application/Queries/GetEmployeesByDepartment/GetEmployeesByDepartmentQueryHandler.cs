namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByDepartment
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;

    public class GetEmployeesByDepartmentQueryHandler : IRequestHandler<GetEmployeesByDepartmentQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService; 
        public GetEmployeesByDepartmentQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetEmployeesByDepartmentQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetByDepartmentAsync(request.DepartmentId);
        }
    } 
}
