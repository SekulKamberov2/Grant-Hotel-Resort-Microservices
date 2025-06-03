namespace GHR.EmployeeManagement.Application.Queries.GetBirthdaysThisMonth
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class GetBirthdaysThisMonthQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
    } 
}
