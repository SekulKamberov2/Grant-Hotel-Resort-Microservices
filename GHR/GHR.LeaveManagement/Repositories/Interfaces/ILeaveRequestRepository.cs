namespace GHR.LeaveManagement.Repositories.Interfaces
{
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    using Google.Protobuf.Collections;

    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveApplication>> GetAllAsync();
        Task<LeaveApplication> GetByIdAsync(int id);
        Task <int> AddAsync(LeaveAppBindingModel request);
        Task UpdateAsync(LeaveApplication request);
        Task DeleteAsync(int id);
        Task<IEnumerable<int>> GetLeaveApplicationsIdsAsync(string status);
        Task<IEnumerable<LeaveApplication>> GetByUserIdAsync(int userId); 
    }

}
