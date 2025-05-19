namespace GHR.DutyManagement.Application.Interfaces
{
    using SharedKernel;
    public interface IDutyRepository
    {
        Task<IdentityResult<bool>> CreateDutyAsync(int employeeId, string task);
        Task<IdentityResult<bool>> GetTaskByNameAsync(string task);
    }
} 
