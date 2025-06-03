namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesSalaryAbove
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class GetEmployeesSalaryAboveQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
        public decimal Salary { get; } 
        public GetEmployeesSalaryAboveQuery(decimal salary) => Salary = salary; 
    } 
}
