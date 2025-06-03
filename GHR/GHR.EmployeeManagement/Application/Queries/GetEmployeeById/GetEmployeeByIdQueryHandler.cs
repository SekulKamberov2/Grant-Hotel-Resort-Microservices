namespace GHR.EmployeeManagement.Application.Queries.GetEmployeeById
{
    using MediatR; 
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel; 
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDTO>>
    {
        private readonly IEmployeeService _employeeService; 
        public GetEmployeeByIdQueryHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<EmployeeDTO>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.GetByIdAsync(request.Id);
        }
    }
}
