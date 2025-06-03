namespace GHR.EmployeeManagement.Application.Queries.GetEmployeeLeaveRequests
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeeById;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel; 
    public class GetEmployeeLeaveRequestsQueryHandler : IRequestHandler<GetEmployeeLeaveRequestsQuery, Result<EmployeeWithAllLeaveRequestsDTO>>
    {
        private readonly IEmployeeService _employeeService;
        public GetEmployeeLeaveRequestsQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<EmployeeWithAllLeaveRequestsDTO>> Handle(GetEmployeeLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetAllLeaveRequestsByUserIdAsync(request.UserId); 
        }
    }
}
