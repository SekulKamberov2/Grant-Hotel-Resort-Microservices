namespace GHR.EmployeeManagement.Application.Queries.Search
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class SearchEmployeesByNameQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
        public string Name { get; }
        public SearchEmployeesByNameQuery(string name) => Name = name;
    } 
}
