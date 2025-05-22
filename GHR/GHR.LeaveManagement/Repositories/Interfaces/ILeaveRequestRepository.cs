namespace GHR.LeaveManagement.Repositories.Interfaces
{
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveApplication>> GetAllAsync();
        Task<LeaveApplication> GetByIdAsync(int id);
        Task <int> AddAsync(LeaveAppBindingModel request);
        Task UpdateAsync(LeaveApplication request);
        Task DeleteAsync(int id); 
    }

}
