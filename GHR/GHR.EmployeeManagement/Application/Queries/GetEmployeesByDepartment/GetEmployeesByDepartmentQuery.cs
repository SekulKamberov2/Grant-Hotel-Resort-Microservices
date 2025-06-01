namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByDepartment
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel;

    public class GetEmployeesByDepartmentQuery : IRequest<IdentityResult<IEnumerable<EmployeeDTO>>>
    {
        public int DepartmentId { get; }

        public GetEmployeesByDepartmentQuery(int departmentId)
        {
            DepartmentId = departmentId;
        }
    } 
}
