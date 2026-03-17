namespace GHR.LeaveManagement.Services
{
    using System.Collections.Generic;

    using Identity.Grpc;
  
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    using GHR.LeaveManagement.Repositories.Interfaces;
    using GHR.LeaveManagement.Services.Interfaces;
    using GHR.SharedKernel;

    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepo;
        private readonly IdentityService.IdentityServiceClient _identityClient;
        private readonly ILogger<LeaveService> _logger;

        public LeaveService(
            ILeaveRepository leaveRepo,
            IdentityService.IdentityServiceClient identityClient,
            ILogger<LeaveService> logger)
        {
            _leaveRepo = leaveRepo;
            _identityClient = identityClient;
            _logger = logger;
        }

        public async Task<Result<int>> SubmitLeaveRequestAsync(LeaveAppBindingModel request)
        {
            try
            {
                var exists = await _leaveRepo.ExistAsync(request.UserId, "Pending");
                if (exists)
                    return Result<int>.Failure("You already have a pending leave request.", 400);

                if (request.StartDate > request.EndDate)
                    return Result<int>.Failure("Start date cannot be after end date.", 400);

                request.TotalDays = (decimal)(request.EndDate - request.StartDate).TotalDays + 1;
                request.Status = "Pending";
                request.RequestedAt = DateTime.UtcNow;

                var newId = await _leaveRepo.AddAsync(request);
                if (newId <= 0)
                    return Result<int>.Failure("Failed to submit leave request. Please try again.", 500);

                return Result<int>.Success(newId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting leave request for user {UserId}", request?.UserId);
                return Result<int>.Failure("An error occurred while submitting your request. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> ApproveLeaveRequestAsync(int requestId, int approverId)
        {
            try
            {
                var request = await _leaveRepo.GetByIdAsync(requestId);
                if (request == null)
                    return Result<bool>.Failure("Leave request not found.", 404);

                if (request.Status != "Pending")
                    return Result<bool>.Failure("Only pending requests can be approved.", 400);

                request.Status = "Approved";
                request.ApproverId = approverId;
                request.DecisionDate = DateTime.UtcNow;

                var updateResult = await _leaveRepo.UpdateAsync(request);
                if (updateResult == 0)
                    return Result<bool>.Failure("Failed to approve request. Please try again.", 500);

                var reduceResult = await _leaveRepo.ReduceUsersRemainingDays(request.TotalDays, request.UserId);
                if (reduceResult == 0)
                {
                    _logger.LogWarning("Approved leave request {RequestId} but could not reduce remaining days for user {UserId}", requestId, request.UserId);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave request {RequestId} by approver {ApproverId}", requestId, approverId);
                return Result<bool>.Failure("An error occurred while approving the request. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> RejectLeaveRequestAsync(int requestId, int approverId)
        {
            try
            {
                var request = await _leaveRepo.GetByIdAsync(requestId);
                if (request == null)
                    return Result<bool>.Failure("Leave request not found.", 404);

                if (request.Status != "Pending")
                    return Result<bool>.Failure("Only pending requests can be rejected.", 400);

                request.Status = "Rejected";
                request.ApproverId = approverId;
                request.DecisionDate = DateTime.UtcNow;

                var updateResult = await _leaveRepo.UpdateAsync(request);
                if (updateResult == 0)
                    return Result<bool>.Failure("Failed to reject request. Please try again.", 500);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave request {RequestId} by approver {ApproverId}", requestId, approverId);
                return Result<bool>.Failure("An error occurred while rejecting the request. Please try again later.", 500);
            }
        }

        public async Task<Result<IEnumerable<LeaveApplication>>> GetAllLeaveRequestsAsync()
        {
            try
            {
                var leaveRequests = await _leaveRepo.GetAllAsync();
                return Result<IEnumerable<LeaveApplication>>.Success(leaveRequests ?? Enumerable.Empty<LeaveApplication>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all leave requests");
                return Result<IEnumerable<LeaveApplication>>.Failure("An error occurred while retrieving leave requests.", 500);
            }
        }

        public async Task<Result<IEnumerable<LeaveApplication>>> GetLeaveRequestsByUserIdAsync(int userId)
        {
            try
            {
                var userLeaveRequests = await _leaveRepo.GetByUserIdAsync(userId);
                return Result<IEnumerable<LeaveApplication>>.Success(userLeaveRequests ?? Enumerable.Empty<LeaveApplication>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave requests for user {UserId}", userId);
                return Result<IEnumerable<LeaveApplication>>.Failure("An error occurred while retrieving leave requests.", 500);
            }
        }

        public async Task<Result<LeaveApplication>> GetLeaveRequestByIdAsync(int requestId)
        {
            try
            {
                var request = await _leaveRepo.GetByIdAsync(requestId);
                if (request == null)
                    return Result<LeaveApplication>.Failure("Leave request not found.", 404);

                return Result<LeaveApplication>.Success(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave request {RequestId}", requestId);
                return Result<LeaveApplication>.Failure("An error occurred while retrieving the leave request.", 500);
            }
        }

        public async Task<Result<bool>> CancelLeaveRequestAsync(int requestId, int userId)
        {
            try
            {
                var request = await _leaveRepo.GetByIdAsync(requestId);
                if (request == null)
                    return Result<bool>.Failure("Leave request not found.", 404);

                if (request.UserId != userId)
                    return Result<bool>.Failure("You can only cancel your own leave requests.", 403);

                if (request.Status != "Pending")
                    return Result<bool>.Failure("Only pending requests can be cancelled.", 400);

                await _leaveRepo.DeleteAsync(requestId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request {RequestId} by user {UserId}", requestId, userId);
                return Result<bool>.Failure("An error occurred while cancelling the request. Please try again later.", 500);
            }
        }

        public async Task<Result<IEnumerable<UserBindingModel>>> GetApplicantsAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return Result<IEnumerable<UserBindingModel>>.Failure("Status must be provided.", 400);

            try
            {
                var userIds = await _leaveRepo.GetLeaveApplicationsIdsAsync(status);
                if (userIds == null || !userIds.Any())
                    return Result<IEnumerable<UserBindingModel>>.Success(Enumerable.Empty<UserBindingModel>());

                var request = new UserIdsRequest();
                request.Ids.AddRange(userIds);
                var reply = await _identityClient.GetUsersByIdsAsync(request);

                var users = reply.Users.Select(u => new UserBindingModel
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
                _logger.LogError(ex, "Error fetching applicants with status '{Status}'", status);
                return Result<IEnumerable<UserBindingModel>>.Failure("An error occurred while retrieving leave applications.", 500);
            }
        }

        public async Task<Result<decimal>> GeUsersRemainingDaysAsync(decimal userId)
        {
            if (userId == 0)
                return Result<decimal>.Failure("Invalid user identifier.", 400);

            try
            {
                var days = await _leaveRepo.GetUsersRemainingDays(userId);
                return Result<decimal>.Success(days > 0 ? days : 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching remaining days for user {UserId}", userId);
                return Result<decimal>.Failure("An error occurred while retrieving your remaining leave days.", 500);
            }
        }
    }
}
