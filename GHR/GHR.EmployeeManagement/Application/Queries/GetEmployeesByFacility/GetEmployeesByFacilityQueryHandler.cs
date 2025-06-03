namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByFacility
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;

    public class GetEmployeesByFacilityQueryHandler : IRequestHandler<GetEmployeesByFacilityQuery, Result<IEnumerable<EmployeeDTO>>>
    {
        private readonly IEmployeeService _employeeService;
        public GetEmployeesByFacilityQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<IEnumerable<EmployeeDTO>>> Handle(GetEmployeesByFacilityQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetByFacilityAsync(request.FacilityId);
        }
    }
}
