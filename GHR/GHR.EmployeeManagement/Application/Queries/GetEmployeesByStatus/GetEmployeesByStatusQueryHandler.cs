namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByStatus
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel; 
    public class GetEmployeesByStatusQueryHandler : IRequestHandler<GetEmployeesByStatusQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService;

        public GetEmployeesByStatusQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
         public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetEmployeesByStatusQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetByStatusAsync(request.Status);
        }
    } 
}
