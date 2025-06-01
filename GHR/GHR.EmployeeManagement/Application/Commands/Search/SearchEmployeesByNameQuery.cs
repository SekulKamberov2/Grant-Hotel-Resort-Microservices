namespace GHR.EmployeeManagement.Application.Commands.Search
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel; 
    public class SearchEmployeesByNameQuery : IRequest<IdentityResult<IEnumerable<EmployeeDTO>>>
    {
        public string Name { get; }
        public SearchEmployeesByNameQuery(string name) => Name = name;
    } 
}
