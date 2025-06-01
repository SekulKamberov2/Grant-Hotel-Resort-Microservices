namespace GHR.EmployeeManagement.Application.Commands.Create
{
    using MediatR;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel;

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, IdentityResult<int>>
    {
        private readonly IEmployeeService _employeeService; 
        public CreateEmployeeCommandHandler(IEmployeeService employeeService) => _employeeService = employeeService; 
        public async Task<IdentityResult<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.CreateAsync(request.Employee);
        }
    } 
}
