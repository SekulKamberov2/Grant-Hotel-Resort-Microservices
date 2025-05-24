namespace GHR.LeaveManagement.Repositories.Interfaces
{
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    public interface ILeaveRepository
    {
        Task<IEnumerable<LeaveApplication>> GetAllAsync();
        Task<LeaveApplication> GetByIdAsync(int id);
        Task <int> AddAsync(LeaveAppBindingModel request);
        Task<int> UpdateAsync(LeaveApplication request);
        Task DeleteAsync(int id);
        Task<IEnumerable<int>> GetLeaveApplicationsIdsAsync(string status);
        Task<bool> ExistAsync(int userId, string status);
        Task<decimal> GetUsersRemainingDays(decimal userId);
        Task<int> ReduceUsersRemainingDays(decimal days, int userId);
    }

}
