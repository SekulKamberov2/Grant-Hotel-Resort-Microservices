namespace GHR.LeaveManagement.Services
{
    using System.Collections.Generic;
    using Identity.Grpc;
    using GHR.SharedKernel;

    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    using GHR.LeaveManagement.Repositories.Interfaces;
    using GHR.LeaveManagement.Services.Interfaces;

    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepo;
        private readonly IdentityService.IdentityServiceClient _identityClient;
        public LeaveService(ILeaveRepository leaveRepo, IdentityService.IdentityServiceClient identityClient)
        {
            _leaveRepo = leaveRepo;
            _identityClient = identityClient; 
        }

        public async Task<IdentityResult<int>> SubmitLeaveRequestAsync(LeaveAppBindingModel request)
        {
            if(request.StartDate > request.EndDate)
                return IdentityResult<int>.Failure("Start date cannot be after end date.");

            request.TotalDays = (decimal)(request.EndDate - request.StartDate).TotalDays + 1;
            request.Status = "Pending";
            request.RequestedAt = DateTime.UtcNow;

            var newId = await _leaveRepo.AddAsync(request);
            if (newId <= 0)
                return IdentityResult<int>.Failure("Failed to add leave request to the database.");

            return IdentityResult<int>.Success(newId);
        }

        public async Task<IdentityResult<bool>> ApproveLeaveRequestAsync(int requestId, int approverId)
        {
            var request = await _leaveRepo.GetByIdAsync(requestId);
            if (request == null)
                return IdentityResult<bool>.Failure("Leave request not found.");  

            if (request.Status != "Pending")
                return IdentityResult<bool>.Failure("Only pending requests can be approved.");  

            request.Status = "Approved";
            request.ApproverId = approverId;
            request.DecisionDate = DateTime.UtcNow;

            try
            {
                await _leaveRepo.UpdateAsync(request);
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure($"Error during update: {ex.Message}"); 
            }

            return IdentityResult<bool>.Success(true);
        }


        public async Task<IdentityResult<bool>> RejectLeaveRequestAsync(int requestId, int approverId)
        {
            var request = await _leaveRepo.GetByIdAsync(requestId);
            if (request == null)
                return IdentityResult<bool>.Failure("Leave request not found.");

            if (request.Status != "Pending")
                return IdentityResult<bool>.Failure("Only pending requests can be rejected.");

            request.Status = "Rejected";
            request.ApproverId = approverId;
            request.DecisionDate = DateTime.UtcNow;

            try
            { 
                await _leaveRepo.UpdateAsync(request);
            }
            catch (Exception ex)
            { 
                return IdentityResult<bool>.Failure($"Error during update: {ex.Message}");
            } 
            return IdentityResult<bool>.Success(true);
        }

        public async Task<IdentityResult<IEnumerable<LeaveApplication>>> GetAllLeaveRequestsAsync()
        {
            try
            { 
                var leaveRequests = await _leaveRepo.GetAllAsync();
                 
                if (leaveRequests == null || !leaveRequests.Any())
                    return IdentityResult<IEnumerable<LeaveApplication>>.Failure("No leave requests found.");   
                 
                return IdentityResult<IEnumerable<LeaveApplication>>.Success(leaveRequests);
            }
            catch (Exception ex)
            { 
                return IdentityResult<IEnumerable<LeaveApplication>>.Failure($"Error fetching leave requests: {ex.Message}");
            }
        }

        public async Task<IdentityResult<IEnumerable<LeaveApplication>>> GetLeaveRequestsByUserIdAsync(int userId)
        {
            try
            { 
                var allLeaveRequests = await _leaveRepo.GetAllAsync(); 
                var userLeaveRequests = allLeaveRequests.Where(lr => lr.UserId == userId).ToList(); 
                if (!userLeaveRequests.Any()) return IdentityResult<IEnumerable<LeaveApplication>>.Failure("No leave requests found for the given user.");
               
                return IdentityResult<IEnumerable<LeaveApplication>>.Success(userLeaveRequests);
            }
            catch (Exception ex)
            { 
                return IdentityResult<IEnumerable<LeaveApplication>>.Failure($"Error fetching leave requests: {ex.Message}");
            }
        }

        public async Task<IdentityResult<LeaveApplication>> GetLeaveRequestByIdAsync(int requestId)
        {
            try
            { 
                var request = await _leaveRepo.GetByIdAsync(requestId); 
                if (request == null) return IdentityResult<LeaveApplication>.Failure("Leave request not found.");
                
                return IdentityResult<LeaveApplication>.Success(request);
            }
            catch (Exception ex)
            { 
                return IdentityResult<LeaveApplication>.Failure($"Error fetching leave request: {ex.Message}");
            }
        }

        public async Task<IdentityResult<bool>> CancelLeaveRequestAsync(int requestId, int userId)
        {
            var request = await _leaveRepo.GetByIdAsync(requestId);
            if (request == null)
                return IdentityResult<bool>.Failure("Leave request not found.");

            if (request.UserId != userId)
                return IdentityResult<bool>.Failure("You can only cancel your own leave.");

            if (request.Status != "Pending")
                return IdentityResult<bool>.Failure("Only pending requests can be cancelled.");

            try
            { 
                await _leaveRepo.DeleteAsync(requestId);
            }
            catch (Exception ex)
            { 
                return IdentityResult<bool>.Failure($"Error during delete: {ex.Message}", 500);
            }
            return IdentityResult<bool>.Success(true);
        }

        public async Task<IdentityResult<IEnumerable<UserBindingModel>>> GetApplicantsAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return IdentityResult<IEnumerable<UserBindingModel>>.Failure("Status must be provided.");

            try
            {
                var userIds = await _leaveRepo.GetLeaveApplicationsIdsAsync(status);
                if (userIds == null || !userIds.Any())
                    return IdentityResult<IEnumerable<UserBindingModel>>.Failure($"No applications ids found with status '{status}'");

                //gRPC call
                var request = new UserIdsRequest();
                request.Ids.AddRange(userIds);
                var reply = await _identityClient.GetUsersByIdsAsync(request);
                var users = reply.Users.Select((User u) => new UserBindingModel
                {
                    Id = u.Id,
                    UserName = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    DateCreated = DateTime.Parse(u.DateCreated)
                }).ToList();


                return IdentityResult<IEnumerable<UserBindingModel>>.Success(users);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<UserBindingModel>>.Failure("An error occurred while retrieving leave applications.");
            }
        }
    }
}
