namespace GHR.EmployeeManagement.Application.Commands.Update
{
    using MediatR;

    using GHR.SharedKernel;
    using GHR.EmployeeManagement.Application.DTOs;
    public class UpdateEmployeeCommand : IRequest<IdentityResult<bool>>
    {
        public int Id { get; set; }
        public UpdateEmployeeDTO Employee { get; set; }

        public UpdateEmployeeCommand(int id, UpdateEmployeeDTO employee)
        {
            Id = id;
            Employee = employee;
        }
    }

}
