namespace GHR.LeaveManagement.Controllers
{
    using GHR.LeaveManagement.DTOs.Input; 
    using GHR.LeaveManagement.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc; 
    public class LeaveController : BaseApiController
    {
        private readonly ILeaveService _leaveService;  
        public LeaveController(ILeaveService leaveService) =>
            _leaveService = leaveService;

        [Authorize(Roles = "EMPLOYEE,MANAGER")]
        [HttpPost]
        public async Task<IActionResult> SubmitLeaveRequest([FromBody] LeaveAppBindingModel request) =>
            AsActionResult(await _leaveService.SubmitLeaveRequestAsync(request));

        [Authorize(Roles = "MANAGER")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromQuery] int approverId) =>
             AsActionResult(await _leaveService.ApproveLeaveRequestAsync(id, approverId));

        [Authorize(Roles = "MANAGER")]
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromQuery] int approverId) =>
             AsActionResult(await _leaveService.RejectLeaveRequestAsync(id, approverId));

        [Authorize(Roles = "EMPLOYEE")]
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromQuery] int userId) =>
              AsActionResult(await _leaveService.CancelLeaveRequestAsync(id, userId));

        [Authorize(Roles = "MANAGER")]
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveRequests() =>
            AsActionResult(await _leaveService.GetAllLeaveRequestsAsync());

        [Authorize(Roles = "MANAGER")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById(int id) => 
            AsActionResult(await _leaveService.GetLeaveRequestByIdAsync(id));

        [Authorize(Roles = "EMPLOYEE")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserLeaveRequests(int userId) =>   
            AsActionResult(await _leaveService.GetLeaveRequestsByUserIdAsync(userId)); 
    }
}
