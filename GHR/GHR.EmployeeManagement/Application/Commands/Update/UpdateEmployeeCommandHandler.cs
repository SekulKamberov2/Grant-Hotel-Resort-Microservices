namespace GHR.EmployeeManagement.Application.Commands.Update
{
    using MediatR;
    using GHR.SharedKernel;
    using GHR.EmployeeManagement.Application.Services;
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, IdentityResult<bool>>
    {
        private readonly IEmployeeService _employeeService; 
        public UpdateEmployeeCommandHandler(IEmployeeService employeeService) => _employeeService = employeeService; 
        public async Task<IdentityResult<bool>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.UpdateAsync(request.Id, request.Employee);
        }
    } 
}
