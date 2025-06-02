using Azure;
using GHR.LeaveManagement.Entities;
using GHR.LeaveManagement.Repositories.Interfaces;
using Grpc.Core;
using Leaverequests.Grpc;
using System.Globalization;

namespace GHR.LeaveManagement.Services
{  
    public class LeaveRequestsGrpcService : LeaverequestsService.LeaverequestsServiceBase
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        public LeaveRequestsGrpcService(ILeaveRequestRepository leaveRequestRepository) => _leaveRequestRepository = leaveRequestRepository;
        public override async Task<LeaveReply> GetLeaveRequestsByEmployee(GetLeaveRequest request, ServerCallContext context)
        {

            IEnumerable<LeaveApplication> leaves = await _leaveRequestRepository.GetByUserIdAsync(request.UserId);
            Console.WriteLine($"Returning {leaves.Count()} leave requests for user {request.UserId}");
            var reply = new LeaveReply();
            reply.Leaves.AddRange(leaves.Select(p => new Leaverequest
            {
                UserId = p.UserId,
                Department = p.Department,
                LeaveTypeId = p.LeaveTypeId,
                StartDate = p.StartDate.ToString("o"),
                EndDate = p.EndDate.ToString("o"),
                TotalDays = p.TotalDays.ToString(CultureInfo.InvariantCulture),
                Reason = p.Reason,
                ApproverId = p.ApproverId ?? 0,
                DecisionDate = p.DecisionDate?.ToString("yyyy-MM-dd") ?? "",
                RequestedAt = p.RequestedAt.ToString("o") 
            }));
            return reply;
        }
    } 
}
