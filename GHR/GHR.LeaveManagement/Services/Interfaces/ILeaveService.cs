namespace GHR.LeaveManagement.Services.Interfaces
{
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    using GHR.SharedKernel;

    public interface ILeaveService
    { 
        Task<IdentityResult<int>> SubmitLeaveRequestAsync(LeaveAppBindingModel request);
        Task<IdentityResult<bool>> ApproveLeaveRequestAsync(int requestId, int approverId);
        Task<IdentityResult<bool>> RejectLeaveRequestAsync(int requestId, int approverId);
        Task<IdentityResult<IEnumerable<LeaveApplication>>> GetAllLeaveRequestsAsync();
        Task<IdentityResult<IEnumerable<LeaveApplication>>> GetLeaveRequestsByUserIdAsync(int userId);
        Task<IdentityResult<LeaveApplication>> GetLeaveRequestByIdAsync(int requestId);
        Task<IdentityResult<bool>> CancelLeaveRequestAsync(int requestId, int userId);
        Task<IdentityResult<IEnumerable<UserBindingModel>>> GetApplicantsAsync(string status);

    } 
}
