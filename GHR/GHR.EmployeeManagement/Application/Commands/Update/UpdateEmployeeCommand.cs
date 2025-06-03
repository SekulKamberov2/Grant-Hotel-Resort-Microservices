namespace GHR.EmployeeManagement.Application.Commands.Update
{
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Domain.Entities;
    using GHR.SharedKernel;
    using MediatR;

    public class UpdateEmployeeCommand : IRequest<Result<Employee>>
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
