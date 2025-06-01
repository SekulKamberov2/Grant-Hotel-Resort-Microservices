namespace GHR.EmployeeManagement.Application.Commands.Delete
{
    using MediatR;

    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel; 
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, IdentityResult<bool>>
    {
        private readonly IEmployeeService _employeeService; 
        public DeleteEmployeeCommandHandler(IEmployeeService employeeService) => _employeeService = employeeService; 
        public async Task<IdentityResult<bool>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _employeeService.DeleteAsync(request.Id);
        }
    } 
}
