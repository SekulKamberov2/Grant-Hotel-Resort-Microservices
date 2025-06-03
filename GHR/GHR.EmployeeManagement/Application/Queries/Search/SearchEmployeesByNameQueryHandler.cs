namespace GHR.EmployeeManagement.Application.Queries.Search
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;

    public class SearchEmployeesByNameQueryHandler : IRequestHandler<SearchEmployeesByNameQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService; 
        public SearchEmployeesByNameQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(SearchEmployeesByNameQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.SearchByNameAsync(request.Name);
        }
    } 
}
