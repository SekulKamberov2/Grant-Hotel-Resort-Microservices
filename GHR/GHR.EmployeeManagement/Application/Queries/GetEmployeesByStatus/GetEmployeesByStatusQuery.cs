namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByStatus
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class GetEmployeesByStatusQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
        public string Status { get; } 
        public GetEmployeesByStatusQuery(string status) => Status = status; 
    } 
}
