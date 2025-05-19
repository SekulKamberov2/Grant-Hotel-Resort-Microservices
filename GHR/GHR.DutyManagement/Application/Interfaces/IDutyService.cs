namespace GHR.DutyManagement.Application.Interfaces
{
    using GHR.SharedKernel;
    public interface IDutyService
    {
        Task<IdentityResult<bool>> AssignDutyAsync(int employeeId, string task);
    }
}
