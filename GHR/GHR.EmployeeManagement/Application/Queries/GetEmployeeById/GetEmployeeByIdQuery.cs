namespace GHR.EmployeeManagement.Application.Queries.GetEmployeeById
{
    using MediatR;

    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel;

    public class GetEmployeeByIdQuery : IRequest<Result<EmployeeDTO>>
    {
        public int Id { get; set; } 
        public GetEmployeeByIdQuery(int id) => Id = id;
    } 
}
