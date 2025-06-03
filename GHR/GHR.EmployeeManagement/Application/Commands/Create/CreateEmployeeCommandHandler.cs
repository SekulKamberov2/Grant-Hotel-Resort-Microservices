namespace GHR.EmployeeManagement.Application.Commands.Create
{
    using MediatR;

    using GHR.EmployeeManagement.Application.Services;
    using GHR.EmployeeManagement.Domain.Entities;
    using GHR.SharedKernel; 
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Employee>>
    {
        private readonly IEmployeeService _employeeService; 
        public CreateEmployeeCommandHandler(IEmployeeService employeeService) => _employeeService = employeeService; 
        public async Task<Result<Employee>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        { 
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.CreateAsync(request.Employee);
        }
    } 
}
