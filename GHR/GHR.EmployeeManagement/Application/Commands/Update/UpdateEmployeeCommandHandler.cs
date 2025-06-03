namespace GHR.EmployeeManagement.Application.Commands.Update
{
    using GHR.EmployeeManagement.Application.Services;
    using GHR.EmployeeManagement.Domain.Entities;
    using GHR.SharedKernel;
    using MediatR;

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<Employee>>
    {
        private readonly IEmployeeService _employeeService; 
        public UpdateEmployeeCommandHandler(IEmployeeService employeeService) => _employeeService = employeeService; 
        public async Task<Result<Employee>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.UpdateAsync(request.Id, request.Employee);
        }
    } 
}
