namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesSalaryAbove
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;

    public class GetEmployeesSalaryAboveQueryHandler : IRequestHandler<GetEmployeesSalaryAboveQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService;
        public GetEmployeesSalaryAboveQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetEmployeesSalaryAboveQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetSalaryAboveAsync(request.Salary);
        }
    }
}
