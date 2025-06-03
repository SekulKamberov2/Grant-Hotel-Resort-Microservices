namespace GHR.EmployeeManagement.Application.Queries.GetEmployeesByFacility
{
    using MediatR;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.SharedKernel;
    public class GetEmployeesByFacilityQuery : IRequest<Result<IEnumerable<EmployeeDTO>>>
    {
        public int FacilityId { get; } 
        public GetEmployeesByFacilityQuery(int facilityId) => FacilityId = facilityId;
    } 
}
