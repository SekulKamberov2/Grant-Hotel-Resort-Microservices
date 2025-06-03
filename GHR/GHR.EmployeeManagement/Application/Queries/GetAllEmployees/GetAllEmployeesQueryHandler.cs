namespace GHR.EmployeeManagement.Application.Queries.GetAllEmployees
{
    using MediatR;

    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;
  
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService; 
        public GetAllEmployeesQueryHandler(IEmployeeService employeeService) =>  _employeeService = employeeService;
        
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetAllAsync();
        }
    } 
}
