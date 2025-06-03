namespace GHR.EmployeeManagement.Application.Queries.GetBirthdaysThisMonth
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;

    public class GetBirthdaysThisMonthQueryHandler : IRequestHandler<GetBirthdaysThisMonthQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService; 
        public GetBirthdaysThisMonthQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetBirthdaysThisMonthQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetBirthdaysThisMonthAsync();
        }
    }
}
