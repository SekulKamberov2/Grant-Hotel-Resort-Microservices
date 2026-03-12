namespace GHR.HelpDesk.Services
{
    using GHR.HelpDesk.DTOs;
    using GHR.HelpDesk.Entities;
    using GHR.HelpDesk.Repositories;
    using GHR.SharedKernel;
    using GHR.SharedKernel.Models;
    using System.Text.Json;

    public interface ITicketService
    {
        Task<Result<TicketWithUserDetailsDto>> GetTicketAsync(int ticketId, CurrentUser currentUser, string? role); 
        Task<Result<IEnumerable<TicketDto>>> GetAllTicketsAsync();
        Task<Result<IEnumerable<TicketDto>>> GetAllUserTicketsAsync(int userId);
        Task<Result<TicketDto>> CreateTicketAsync(TicketDto ticket);
        Task<Result<bool>> UpdateTicketAsync(TicketDto ticket);
        Task<Result<bool>> DeleteTicketAsync(int ticketId);
        Task<Result<IEnumerable<TicketLogDto>>> GetTicketLogsAsync(int ticketId);
        Task<Result<bool>> AddTicketLogAsync(TicketLogDto log);
        Task<Result<bool>> AssignTicketAsync(int ticketId, int staffId);
        Task<Result<bool>> UpdateStatusAsync(int ticketId, int statusId); 
        Task<Result<IEnumerable<TicketDto>>> GetTicketsByStatusAsync(int statusId);
        Task<Result<IEnumerable<TicketDto>>> GetTicketsByStaffAsync(int staffId);
        Task<Result<IEnumerable<TicketDto>>> GetTicketsByDateRangeAsync(DateTime startDate, DateTime endDate);  
        Task<Result<bool>> AddCommentAsync(CommentDto comment);
        Task<Result<IEnumerable<CommentDto>>> GetCommentsAsync(int ticketId); 
        Task<Result<Dictionary<int, int>>> GetTicketCountGroupedByStatusAsync(); 
        Task<Result<bool>> ReopenTicketAsync(int ticketId); 
        Task<Result<IEnumerable<TicketDto>>> GetTicketsByPriorityAsync(int priorityId); 
        Task<Result<bool>> BulkUpdateStatusAsync(IEnumerable<int> ticketIds, int statusId);
        Task<Result<PagedResult<TicketDto>>> GetFilteredTicketsAsync(TicketFilterDto filter, int page, int pageSize); 
    }

    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketService(ITicketRepository ticketRepository) => _ticketRepository = ticketRepository; 

        public async Task<Result<TicketWithUserDetailsDto>> GetTicketAsync(int ticketId, CurrentUser currentUser, string? role)
        {     
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId); 
                if (ticket == null)
                    return Result<TicketWithUserDetailsDto>.Failure("Ticket not found.", 404);
               
                if (ticket.UserId != currentUser.Id && role != "HD ADMIN")
                    return Result<TicketWithUserDetailsDto>.Failure("Unauthorized access to this ticket.", 401);
                
                var result = new TicketWithUserDetailsDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    UserId = ticket.UserId,
                    StaffId = ticket.StaffId,
                    DepartmentId = ticket.DepartmentId,
                    LocationId = ticket.LocationId,
                    CategoryId = ticket.CategoryId,
                    PriorityId = ticket.PriorityId,
                    StatusId = ticket.StatusId,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,

                    UserName = currentUser.UserName,
                    Email = currentUser.Email,
                    PhoneNumber = currentUser.PhoneNumber 
                };  
                return Result<TicketWithUserDetailsDto>.Success(result);
            }
            catch
            {
                return Result<TicketWithUserDetailsDto>.Failure("An error occurred while retrieving the ticket. Please try again later.", 500);
            }
        }


        public async Task<Result<IEnumerable<TicketDto>>> GetAllTicketsAsync()
        {
            try
            {
                var tickets = await _ticketRepository.GetAllAsync();
                var result = tickets.Select(ticket => new TicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    UserId = ticket.UserId,
                    StaffId = ticket.StaffId,
                    DepartmentId = ticket.DepartmentId,
                    LocationId = ticket.LocationId,
                    CategoryId = ticket.CategoryId,
                    PriorityId = ticket.PriorityId,
                    StatusId = ticket.StatusId,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt
                });
                return Result<IEnumerable<TicketDto>>.Success(result);
            }
            catch
            {
                return Result<IEnumerable<TicketDto>>.Failure("An error occurred while fetching tickets. Please try again later.", 500);
            }
        } 
        public async Task<Result<IEnumerable<TicketDto>>> GetAllUserTicketsAsync(int userId)
        { 
            try
            {
                var tickets = await _ticketRepository.GetAllUserTicketsAsync(userId);
                var result = tickets.Select(ticket => new TicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    UserId = ticket.UserId,
                    StaffId = ticket.StaffId,
                    DepartmentId = ticket.DepartmentId,
                    LocationId = ticket.LocationId,
                    CategoryId = ticket.CategoryId,
                    PriorityId = ticket.PriorityId,
                    StatusId = ticket.StatusId,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt
                });
                return Result<IEnumerable<TicketDto>>.Success(result);
            }
            catch
            {
                return Result<IEnumerable<TicketDto>>.Failure("An error occurred while fetching tickets. Please try again later.", 500);
            }
        }
         

        public async Task<Result<TicketDto>> CreateTicketAsync(TicketDto ticketDto)
        {
            try
            {
                if (ticketDto == null)
                    return Result<TicketDto>.Failure("Invalid ticket data.", 400);

                var entity = new Ticket
                { 
                    Title = ticketDto.Title,
                    Description = ticketDto.Description,
                    UserId = ticketDto.UserId,
                    StaffId = ticketDto.StaffId,
                    DepartmentId = ticketDto.DepartmentId,
                    LocationId = ticketDto.LocationId,
                    CategoryId = ticketDto.CategoryId,
                    PriorityId = ticketDto.PriorityId,
                    StatusId = ticketDto.StatusId,
                    CreatedAt = ticketDto.CreatedAt,
                    TicketTypeId = ticketDto.TicketTypeId  
                };

                var id = await _ticketRepository.CreateAsync(entity);
                if(id == 0)
                    return Result<TicketDto>.Failure($"Failed to create the ticket", 500);

                ticketDto.Id = id;  
                return Result<TicketDto>.Success(ticketDto);
            }
            catch(Exception e)
            {
                return Result<TicketDto>.Failure($"Failed to create the ticket. Please try again later.{e.Message}", 500);
            }
        }

        public async Task<Result<bool>> UpdateTicketAsync(TicketDto ticketDto)
        {
            try
            {
                var existing = await _ticketRepository.GetByIdAsync(ticketDto.Id);
                if (existing == null)
                    return Result<bool>.Failure("Ticket not found.", 404);

                var entity = new Ticket
                {
                    Id = ticketDto.Id,
                    Title = ticketDto.Title,
                    Description = ticketDto.Description,
                    UserId = ticketDto.UserId,
                    StaffId = ticketDto.StaffId,
                    DepartmentId = ticketDto.DepartmentId,
                    LocationId = ticketDto.LocationId,
                    CategoryId = ticketDto.CategoryId,
                    PriorityId = ticketDto.PriorityId,
                    StatusId = ticketDto.StatusId,
                    TicketTypeId = ticketDto.TicketTypeId,
                    CreatedAt = ticketDto.CreatedAt,
                    UpdatedAt = ticketDto.UpdatedAt
                };

                var rows = await _ticketRepository.UpdateAsync(entity);
                if (rows == 0)
                    return Result<bool>.Failure("Failed to update the ticket.", 500);

                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("Failed to update the ticket. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> DeleteTicketAsync(int ticketId)
        {
            try
            {
                var existing = await _ticketRepository.GetByIdAsync(ticketId);
                if (existing == null)
                    return Result<bool>.Failure("Ticket not found.", 404);

                var rows = await _ticketRepository.DeleteAsync(ticketId);
                if (rows == 0)
                    return Result<bool>.Failure("An error occurred while deleting the ticket.", 500);

                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("An error occurred while deleting the ticket. Please try again later.", 500);
            }
        }

        public async Task<Result<IEnumerable<TicketLogDto>>> GetTicketLogsAsync(int ticketId)
        {
            try
            {
                var logs = await _ticketRepository.GetLogsAsync(ticketId);
                var dtoLogs = logs.Select(log => new TicketLogDto
                {
                    Id = log.Id,
                    TicketId = log.TicketId,
                    Comment = log.Comment,
                    CreatedBy = log.CreatedBy,
                    CreatedByRole = log.CreatedByRole,
                    CreatedAt = log.CreatedAt
                });

                return Result<IEnumerable<TicketLogDto>>.Success(dtoLogs);
            }
            catch
            {
                return Result<IEnumerable<TicketLogDto>>.Failure("An error occurred while fetching ticket logs. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> AddTicketLogAsync(TicketLogDto logDto)
        {
            try
            { 
                if (logDto == null)
                    return Result<bool>.Failure("Invalid log data.", 400);

                var log = new TicketLog
                { 
                    TicketId = logDto.TicketId,
                    Comment = logDto.Comment,
                    CreatedBy = logDto.CreatedBy,
                    CreatedByRole = logDto.CreatedByRole 
                };

                var result = await _ticketRepository.AddLogAsync(log);
                    if(result == 0) return Result<bool>.Failure("An error occurred while adding the ticket log.", 500);

                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("An error occurred while adding the ticket log. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> AssignTicketAsync(int ticketId, int staffId)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                if (ticket == null)
                    return Result<bool>.Failure("Ticket not found.", 404);

                var result = await _ticketRepository.AssignStaffAsync(ticketId, staffId);
                if(result == 0) 
                    return Result<bool>.Failure("Failed to assign the ticket.", 500);

                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("Failed to assign the ticket. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> UpdateStatusAsync(int ticketId, int statusId)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                if (ticket == null) return Result<bool>.Failure("Ticket not found.", 404);

                var result = await _ticketRepository.UpdateStatusAsync(ticketId, statusId);
                if(result == 0) 
                    return Result<bool>.Failure("Failed to update the ticket status.", 500);

                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("Failed to update the ticket status. Please try again later.", 500);
            }
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetTicketsByStatusAsync(int statusId)
        {
            try
            {
                var tickets = await _ticketRepository.GetByStatusAsync(statusId);
                var result = tickets.Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    UserId = t.UserId,
                    StaffId = t.StaffId,
                    DepartmentId = t.DepartmentId,
                    LocationId = t.LocationId,
                    CategoryId = t.CategoryId,
                    PriorityId = t.PriorityId,
                    StatusId = t.StatusId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                });
                return Result<IEnumerable<TicketDto>>.Success(result);
            }
            catch
            {
                return Result<IEnumerable<TicketDto>>.Failure("Failed to get tickets by status.", 500);
            }
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetTicketsByStaffAsync(int staffId)
        {
            try
            {
                var tickets = await _ticketRepository.GetByStaffAsync(staffId);
                var result = tickets.Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    UserId = t.UserId,
                    StaffId = t.StaffId,
                    DepartmentId = t.DepartmentId,
                    LocationId = t.LocationId,
                    CategoryId = t.CategoryId,
                    PriorityId = t.PriorityId,
                    StatusId = t.StatusId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                });
                return Result<IEnumerable<TicketDto>>.Success(result);
            }
            catch
            {
                return Result<IEnumerable<TicketDto>>.Failure("Failed to get tickets by staff.", 500);
            }
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetTicketsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var tickets = await _ticketRepository.GetByDateRangeAsync(startDate, endDate); //unsuccess throws ecception

                var result = tickets.Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    UserId = t.UserId,
                    StaffId = t.StaffId,
                    DepartmentId = t.DepartmentId,
                    LocationId = t.LocationId,
                    CategoryId = t.CategoryId,
                    PriorityId = t.PriorityId,
                    StatusId = t.StatusId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                });
                return Result<IEnumerable<TicketDto>>.Success(result);
            }
            catch
            {
                return Result<IEnumerable<TicketDto>>.Failure("Failed to get tickets by date range.", 500);
            }
        }

        public async Task<Result<bool>> AddCommentAsync(CommentDto commentDto)
        {
            try
            {
                if (commentDto == null)
                    return Result<bool>.Failure("Invalid comment data.", 400);

                var comment = new Comment
                {
                    TicketId = commentDto.TicketId,
                    Text = commentDto.Text,
                    CreatedAt = commentDto.CreatedAt,
                    CreatedByUserId = commentDto.CreatedByUserId
                };

                await _ticketRepository.AddCommentAsync(comment);   //in case of unsuccess throws an exception
                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("Failed to add comment.", 500);
            }
        }

        public async Task<Result<IEnumerable<CommentDto>>> GetCommentsAsync(int ticketId)
        {
            try
            {
                var comments = await _ticketRepository.GetCommentsAsync(ticketId);  
                var result = comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    TicketId = c.TicketId,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    CreatedByUserId = c.CreatedByUserId
                });
                return Result<IEnumerable<CommentDto>>.Success(result);
            }
            catch
            {
                return Result<IEnumerable<CommentDto>>.Failure("Failed to get comments.", 500);
            }
        }

        public async Task<Result<Dictionary<int, int>>> GetTicketCountGroupedByStatusAsync()
        {
            try
            {
                var counts = await _ticketRepository.GetTicketCountGroupedByStatusAsync();
                return Result<Dictionary<int, int>>.Success(counts);
            }
            catch
            {
                return Result<Dictionary<int, int>>.Failure("Failed to get ticket counts by status.", 500);
            }
        }

        public async Task<Result<bool>> ReopenTicketAsync(int ticketId)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                if (ticket == null)
                    return Result<bool>.Failure("Ticket not found.", 404);
                 
                await _ticketRepository.UpdateStatusAsync(ticketId, 1);
                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("Failed to reopen ticket.", 500);
            }
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetTicketsByPriorityAsync(int priorityId)
        {
            try
            {
                var tickets = await _ticketRepository.GetByPriorityAsync(priorityId);
                var result = tickets.Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    UserId = t.UserId,
                    StaffId = t.StaffId,
                    DepartmentId = t.DepartmentId,
                    LocationId = t.LocationId,
                    CategoryId = t.CategoryId,
                    PriorityId = t.PriorityId,
                    StatusId = t.StatusId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                });
                return Result<IEnumerable<TicketDto>>.Success(result);
            }
            catch
            {
                return Result<IEnumerable<TicketDto>>.Failure("Failed to get tickets by priority.", 500);
            }
        }

        public async Task<Result<bool>> BulkUpdateStatusAsync(IEnumerable<int> ticketIds, int statusId)
        {
            try
            {
                if (ticketIds == null || !ticketIds.Any())
                    return Result<bool>.Failure("No ticket IDs provided.", 400);

                var rows = await _ticketRepository.BulkUpdateStatusAsync(ticketIds, statusId);
                if(rows == 0) 
                    return Result<bool>.Failure("Failed to bulk update ticket statuses.", 500);

                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure("Failed to bulk update ticket statuses.", 500);
            }
        }

        public async Task<Result<PagedResult<TicketDto>>> GetFilteredTicketsAsync(TicketFilterDto filter, int page, int pageSize)
        {
            try
            {
                var (tickets, totalCount) = await _ticketRepository.GetFilteredTicketsPagedAsync(filter, page, pageSize);

                var result = tickets.Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    UserId = t.UserId,
                    StaffId = t.StaffId,
                    DepartmentId = t.DepartmentId,
                    LocationId = t.LocationId,
                    CategoryId = t.CategoryId,
                    PriorityId = t.PriorityId,
                    StatusId = t.StatusId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt 
                });

                var pagedResult = new PagedResult<TicketDto>
                {
                    Items = result,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };

                return Result<PagedResult<TicketDto>>.Success(pagedResult);
            }
            catch
            {
                return Result<PagedResult<TicketDto>>.Failure("Failed to get filtered tickets.", 500);
            }
        }
         
    }
}
