namespace GHR.LeaveManagement.Services
{
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    using GHR.LeaveManagement.Repositories.Interfaces;
    using GHR.LeaveManagement.Services.Interfaces;
    using GHR.SharedKernel;
    using Identity.Grpc;
    using System.Collections.Generic;

    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepo;
        private readonly IdentityService.IdentityServiceClient _identityClient;
        public LeaveService(ILeaveRepository leaveRepo, IdentityService.IdentityServiceClient identityClient)
        {
            _leaveRepo = leaveRepo;
            _identityClient = identityClient; 
        }

        public async Task<Result<int>> SubmitLeaveRequestAsync(LeaveAppBindingModel request)
        {
            var exists = await _leaveRepo.ExistAsync(request.UserId, "Pending");
            if (exists) return Result<int>.Failure("You've already have Pending request to leave.");

            if (request.StartDate > request.EndDate)
                return Result<int>.Failure("Start date cannot be after end date.");

            request.TotalDays = (decimal)(request.EndDate - request.StartDate).TotalDays + 1;
            request.Status = "Pending";
            request.RequestedAt = DateTime.UtcNow;

            var newId = await _leaveRepo.AddAsync(request);
            if (newId <= 0)
                return Result<int>.Failure("Failed to add leave request to the database.");

            return Result<int>.Success(newId);
        }

        public async Task<Result<bool>> ApproveLeaveRequestAsync(int requestId, int approverId)
        { 
            var request = await _leaveRepo.GetByIdAsync(requestId);
            if (request == null)
                return Result<bool>.Failure("Leave request not found.");
             
            if (request.Status != "Pending")
                return Result<bool>.Failure("Only pending requests can be approved.");
             
            request.Status = "Approved";
            request.ApproverId = approverId;
            request.DecisionDate = DateTime.UtcNow;
             
            var result = await _leaveRepo.UpdateAsync(request);
            if (result == 0)
                return Result<bool>.Failure("Approve fails. Please try again.");
             
            var success = await _leaveRepo.ReduceUsersRemainingDays(request.TotalDays, request.UserId);
            if (success == 0)
                return Result<bool>.Failure("Unable to reduce the user's remaining days. Please try again.");
             
            return Result<bool>.Success(true);
        }


        public async Task<Result<bool>> RejectLeaveRequestAsync(int requestId, int approverId)
        {
            var request = await _leaveRepo.GetByIdAsync(requestId);
            if (request == null)
                return Result<bool>.Failure("Leave request not found.");

            if (request.Status != "Pending")
                return Result<bool>.Failure("Only pending requests can be rejected.");

            request.Status = "Rejected";
            request.ApproverId = approverId;
            request.DecisionDate = DateTime.UtcNow;

            try
            { 
                await _leaveRepo.UpdateAsync(request);
            }
            catch (Exception ex)
            { 
                return Result<bool>.Failure($"Error during update: {ex.Message}");
            } 
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<LeaveApplication>>> GetAllLeaveRequestsAsync()
        {
            try
            { 
                var leaveRequests = await _leaveRepo.GetAllAsync();
                 
                if (leaveRequests == null || !leaveRequests.Any())
                    return Result<IEnumerable<LeaveApplication>>.Failure("No leave requests found.");   
                 
                return Result<IEnumerable<LeaveApplication>>.Success(leaveRequests);
            }
            catch (Exception ex)
            { 
                return Result<IEnumerable<LeaveApplication>>.Failure($"Error fetching leave requests: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<LeaveApplication>>> GetLeaveRequestsByUserIdAsync(int userId)
        {
            try
            { 
                var allLeaveRequests = await _leaveRepo.GetAllAsync(); 
                var userLeaveRequests = allLeaveRequests.Where(lr => lr.UserId == userId).ToList(); 
                if (!userLeaveRequests.Any()) 
                    return Result<IEnumerable<LeaveApplication>>.Failure("No leave requests found for the given user.");
               
                return Result<IEnumerable<LeaveApplication>>.Success(userLeaveRequests);
            }
            catch (Exception ex)
            { 
                return Result<IEnumerable<LeaveApplication>>.Failure($"Error fetching leave requests: {ex.Message}");
            }
        }

        public async Task<Result<LeaveApplication>> GetLeaveRequestByIdAsync(int requestId)
        {
            try
            { 
                var request = await _leaveRepo.GetByIdAsync(requestId); 
                if (request == null) return Result<LeaveApplication>.Failure("Leave request not found.");
                
                return Result<LeaveApplication>.Success(request);
            }
            catch (Exception ex)
            { 
                return Result<LeaveApplication>.Failure($"Error fetching leave request: {ex.Message}");
            }
        }

        public async Task<Result<bool>> CancelLeaveRequestAsync(int requestId, int userId)
        {
            var request = await _leaveRepo.GetByIdAsync(requestId);
            if (request == null)
                return Result<bool>.Failure("Leave request not found.");

            if (request.UserId != userId)
                return Result<bool>.Failure("You can only cancel your own leave.");

            if (request.Status != "Pending")
                return Result<bool>.Failure("Only pending requests can be cancelled.");

            try
            { 
                await _leaveRepo.DeleteAsync(requestId);
            }
            catch (Exception ex)
            { 
                return Result<bool>.Failure($"Error during delete: {ex.Message}");
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<UserBindingModel>>> GetApplicantsAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return Result<IEnumerable<UserBindingModel>>.Failure("Status must be provided.");

            try
            {
                var userIds = await _leaveRepo.GetLeaveApplicationsIdsAsync(status);
                if (userIds == null || !userIds.Any())
                    return Result<IEnumerable<UserBindingModel>>.Failure($"No applications ids found with status '{status}'");

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


                return Result<IEnumerable<UserBindingModel>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserBindingModel>>.Failure("An error occurred while retrieving leave applications.");
            }
        }

        public async Task<Result<decimal>> GeUsersRemainingDaysAsync(decimal userId)
        {
            if (userId == 0) return Result<decimal>.Failure("It's not you.");   
            try
            {
                var days = await _leaveRepo.GetUsersRemainingDays(userId); 
                return Result<decimal>.Success((days > 0) ? days : 0);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Error during calculations: {ex.Message}");
            }  
        }
    }
}
