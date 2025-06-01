namespace GHR.EmployeeManagement.Application.Commands.Create
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class CreateEmployeeCommand : IRequest<IdentityResult<int>>
    {
        public CreateEmployeeDTO Employee { get; set; } 
        public CreateEmployeeCommand(CreateEmployeeDTO employee) => Employee = employee;
    } 
}
