namespace GHR.EmployeeManagement.Application.Queries.GetEmployeeLeaveRequests
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel;
    public class GetEmployeeLeaveRequestsQuery : IRequest<IdentityResult<EmployeeWithAllLeaveRequestsDTO>>
    {
        public int UserId { get; set; }
        public GetEmployeeLeaveRequestsQuery(int userId) => UserId = userId;
    }
}
