namespace GHR.EmployeeManagement.Application.Commands.IncreaseSalary
{
    using MediatR;
    using GHR.EmployeeManagement.Application.Services;
    using GHR.SharedKernel; 
    public class IncreaseSalaryCommandHandler : IRequestHandler<IncreaseSalaryCommand, Result<bool>>
    {
        private readonly IEmployeeService _employeeService; 
        public IncreaseSalaryCommandHandler(IEmployeeService employeeService) => _employeeService = employeeService;
        public async Task<Result<bool>> Handle(IncreaseSalaryCommand request, CancellationToken cancellationToken)
        {
            return await _employeeService.IncreaseSalaryHiredBeforeAsync(request.Years, request.Percentage);
        }
    } 
}
