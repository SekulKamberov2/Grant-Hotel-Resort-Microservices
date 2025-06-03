namespace GHR.EmployeeManagement.Application.Queries.GetAllEmployees
{
    using MediatR;

    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel;
 
    public class GetAllEmployeesQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
    }

}
