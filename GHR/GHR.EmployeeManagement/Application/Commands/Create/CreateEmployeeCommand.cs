namespace GHR.EmployeeManagement.Application.Commands.Create
{
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Domain.Entities;
    using GHR.SharedKernel;
    using MediatR;
    using System;

    public class CreateEmployeeCommand : IRequest<Result<Employee>>
    {
        public CreateEmployeeDTO Employee { get; set; }

        public CreateEmployeeCommand(CreateEmployeeDTO employee)
        {
            Employee = employee;
        }
    } 
}
