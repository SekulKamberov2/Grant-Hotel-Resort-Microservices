namespace GHR.LeaveManagement.Services.Interfaces
{
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    using GHR.SharedKernel;

    public interface ILeaveService
    { 
        Task<Result<int>> SubmitLeaveRequestAsync(LeaveAppBindingModel request);
        Task<Result<bool>> ApproveLeaveRequestAsync(int requestId, int approverId);
        Task<Result<bool>> RejectLeaveRequestAsync(int requestId, int approverId);
        Task<Result<IEnumerable<LeaveApplication>>> GetAllLeaveRequestsAsync();
        Task<Result<IEnumerable<LeaveApplication>>> GetLeaveRequestsByUserIdAsync(int userId);
        Task<Result<LeaveApplication>> GetLeaveRequestByIdAsync(int requestId);
        Task<Result<bool>> CancelLeaveRequestAsync(int requestId, int userId);
        Task<Result<IEnumerable<UserBindingModel>>> GetApplicantsAsync(string status);
        Task<Result<decimal>> GeUsersRemainingDaysAsync(decimal userId);
    } 
}
