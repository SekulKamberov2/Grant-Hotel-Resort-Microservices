namespace GHR.HelpDesk.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using GHR.HelpDesk.DTOs; 
    using GHR.HelpDesk.Services;
    using GHR.SharedKernel.Extensions; 
 
    public class TicketsController : BaseApiController
    {
        private readonly ITicketService _ticketService; 
        public TicketsController(ITicketService ticketService) => _ticketService = ticketService;
         
        //[Authorize(Roles = "HD MANAGER")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets() =>
            AsActionResult(await _ticketService.GetAllTicketsAsync());

        //only HD ADMIN & The owner of the ticket!
        //[Authorize]
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicket(int ticketId) =>
            AsActionResult(await _ticketService.GetTicketAsync(
                ticketId, User.ToCurrentUser(), User.GetRole()));  //User.TryGetUserIdAsInt(out int userId) ? userId : null, User.GetRole()));
         
        //customer role!
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketDto ticket) =>
            AsActionResult(await _ticketService.CreateTicketAsync(ticket));

        //your own ticket update + customer role!
        //[Authorize(Roles = "HD MANAGER")]
        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTicket(int ticketId, [FromBody] TicketDto ticket)
        {
            if (ticketId != ticket.Id) return BadRequest();
            return AsActionResult(await _ticketService.UpdateTicketAsync(ticket));
        }
         
        //customer role!
        // DELETE: api/tickets/{ticketId}
        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTicket(int ticketId) =>
            AsActionResult(await _ticketService.DeleteTicketAsync(ticketId));

        //[Authorize(Roles = "HD ADMIN")]
        // GET: api/tickets/{ticketId}/logs
        [HttpGet("{ticketId}/logs")]
        public async Task<IActionResult>GetTicketLogs(int ticketId) =>
            AsActionResult(await _ticketService.GetTicketLogsAsync(ticketId));

        //[Authorize(Roles = "HD MANAGER")]
        // POST: api/tickets/{ticketId}/logs
        [HttpPost("{ticketId}/logs")]
        public async Task<IActionResult> AddTicketLog(int ticketId, [FromBody] TicketLogDto log) =>
            AsActionResult(await _ticketService.AddTicketLogAsync(log));

        //[Authorize(Roles = "HD MANAGER")]
        // PATCH: api/tickets/{ticketId}/assign/{staffId}
        [HttpPatch("{ticketId}/assign/{staffId}")]
        public async Task<IActionResult> AssignTicketToStaff(int ticketId, int staffId) =>
             AsActionResult(await _ticketService.AssignTicketAsync(ticketId, staffId));

        //[Authorize(Roles = "HD AGENT")]
        // PATCH: api/tickets/{ticketId}/status/{statusId}
        [HttpPatch("{ticketId}/status/{statusId}")]
        public async Task<IActionResult> UpdateTicketStatus(int ticketId, int statusId) =>
            AsActionResult(await _ticketService.UpdateStatusAsync(ticketId, statusId));

        //[Authorize(Roles = "HD AGENT")]
        // GET: api/tickets/status/{statusId}
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetTicketsByStatus(int statusId) =>
            AsActionResult(await _ticketService.GetTicketsByStatusAsync(statusId));

        //[Authorize(Roles = "HD MANAGER")]
        // GET: api/tickets/assigned/{staffId}
        [HttpGet("assigned/{staffId}")]
        public async Task<IActionResult> GetTicketsByStaff(int staffId) =>
            AsActionResult(await _ticketService.GetTicketsByStaffAsync(staffId));

        //[Authorize(Roles = "HD MANAGER")]
        // GET: api/tickets/date-range?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        [HttpGet("date-range")]
        public async Task<IActionResult> GetTicketsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate) =>
            AsActionResult(await _ticketService.GetTicketsByDateRangeAsync(startDate, endDate));

        //[Authorize(Roles = "HD AGENT")]
        // POST: api/tickets/{ticketId}/comments
        [HttpPost("{ticketId}/comments")]
        public async Task<IActionResult> AddComment(int ticketId, [FromBody] CommentDto comment)
        {
            if (comment == null || comment.TicketId != ticketId) return BadRequest("Invalid comment data.");
            return AsActionResult(await _ticketService.AddCommentAsync(comment));
        }

        //[AllowAnonymous]
        // GET: api/tickets/{ticketId}/comments
        [HttpGet("{ticketId}/comments")]
        public async Task<IActionResult> GetComments(int ticketId) =>
            AsActionResult(await _ticketService.GetCommentsAsync(ticketId));
         
        //[Authorize(Roles = "HD MANAGER")]
        // GET: api/tickets/count-by-status
        [HttpGet("count-by-status")]
        public async Task<IActionResult> GetTicketCountByStatus() =>
            AsActionResult(await _ticketService.GetTicketCountGroupedByStatusAsync());

        //[Authorize(Roles = "HD MANAGER")]
        // PATCH: api/tickets/{ticketId}/reopen
        [HttpPatch("{ticketId}/reopen")]
        public async Task<IActionResult> ReopenTicket(int ticketId) =>
            AsActionResult(await _ticketService.ReopenTicketAsync(ticketId));

        //[Authorize(Roles = "HD AGENT")]
        // GET: api/tickets/priority/{priorityId}
        [HttpGet("priority/{priorityId}")]
        public async Task<IActionResult> GetTicketsByPriority(int priorityId) =>
            AsActionResult(await _ticketService.GetTicketsByPriorityAsync(priorityId));

        //updating the status of multiple items all at once
        // PATCH: api/tickets/bulk-status-update
        //[Authorize(Roles = "HD MANAGER")]
        [HttpPatch("bulk-status-update")]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkStatusUpdateDto updateDto)
        {
            if (updateDto == null || updateDto.TicketIds == null || !updateDto.TicketIds.Any())
                return BadRequest("Invalid data for bulk update.");
            return AsActionResult(await _ticketService.BulkUpdateStatusAsync(updateDto.TicketIds, updateDto.StatusId));
        } 

        // GET: api/tickets/filter?page=1&pageSize=20&statusId=1&staffId=5&priorityId=2&createdAfter=2023-01-01&searchTerm=wifi
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredTickets(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? userId = null,
            [FromQuery] int? staffId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? locationId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? priorityId = null,
            [FromQuery] int? statusId = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null,
            [FromQuery] string? searchTerm = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;

            var filter = new TicketFilterDto
            {
                UserId = userId,
                StaffId = staffId,
                DepartmentId = departmentId,
                LocationId = locationId,
                CategoryId = categoryId,
                PriorityId = priorityId,
                StatusId = statusId,
                CreatedAfter = createdAfter,
                CreatedBefore = createdBefore,
                SearchTerm = searchTerm
            }; 
            return AsActionResult(await _ticketService.GetFilteredTicketsAsync(filter, page, pageSize)); 
        } 
    }
}
