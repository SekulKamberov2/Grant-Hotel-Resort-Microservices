namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByManager
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class GetEmployeesByManagerQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
        public int ManagerId { get; } 
        public GetEmployeesByManagerQuery(int managerId) => ManagerId = managerId;
    } 
}
