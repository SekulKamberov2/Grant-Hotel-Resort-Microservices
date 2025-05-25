namespace GHR.HelpDesk.Repositories
{
    using System.Data;

    using Dapper;

    using GHR.HelpDesk.DTOs;
    using GHR.HelpDesk.Entities;  
    using GHR.SharedKernel.Exceptions;
    public interface ITicketRepository
    {
        Task<Ticket> GetByIdAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<int> CreateAsync(Ticket ticket);
        Task<int> UpdateAsync(Ticket ticket);
        Task<int> DeleteAsync(int ticketId);
        Task<IEnumerable<TicketLog>> GetLogsAsync(int ticketId);
        Task<int> AddLogAsync(TicketLog log);
        Task<int> AssignStaffAsync(int ticketId, int staffId);
        Task<int> UpdateStatusAsync(int ticketId, int statusId); 
        Task<IEnumerable<Ticket>> GetByStatusAsync(int statusId);
        Task<IEnumerable<Ticket>> GetByStaffAsync(int staffId);
        Task<IEnumerable<Ticket>> GetByDateRangeAsync(DateTime startDate, DateTime endDate); 
        Task<int> AddCommentAsync(Comment comment);
        Task<IEnumerable<Comment>> GetCommentsAsync(int ticketId); 
        Task<Dictionary<int, int>> GetTicketCountGroupedByStatusAsync(); 
        Task<IEnumerable<Ticket>> GetByPriorityAsync(int priorityId); 
        Task<int> BulkUpdateStatusAsync(IEnumerable<int> ticketIds, int statusId);
        Task<(IEnumerable<Ticket>, int totalCount)> GetFilteredTicketsPagedAsync(TicketFilterDto filter, int page, int pageSize);
    }   

    public class TicketRepository : ITicketRepository
    {
        private readonly IDbConnection _dbConnection; 
        public TicketRepository(IDbConnection dbConnection) =>
            _dbConnection = dbConnection;

        public Task<Ticket?> GetByIdAsync(int ticketId)
        {
            const string sql = @"SELECT * FROM Tickets WHERE Id = @TicketId";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryFirstOrDefaultAsync<Ticket>(sql, new { TicketId = ticketId }),
                "Failed to retrieve ticket by Id.");
        }

        public Task<IEnumerable<Ticket>> GetAllAsync()
        {
            const string sql = @"SELECT * FROM Tickets ORDER BY CreatedAt DESC";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryAsync<Ticket>(sql),
                "Failed to retrieve all tickets.");
        }

        public Task<int> CreateAsync(Ticket ticket)
        {
            const string sql = @"
                INSERT INTO Tickets (Title, Description, UserId, StaffId, DepartmentId, LocationId, 
                                     CategoryId, PriorityId, StatusId, TicketTypeId, CreatedAt)
                VALUES (@Title, @Description, @UserId, @StaffId, @DepartmentId, @LocationId,
                        @CategoryId, @PriorityId, @StatusId, @TicketTypeId, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteScalarAsync<int>(sql, ticket),
                "Failed to create ticket.");
        }

        public Task<int> UpdateAsync(Ticket ticket)
        {
            const string sql = @"
                UPDATE Tickets
                SET Title = @Title,
                    Description = @Description,
                    StaffId = @StaffId,
                    DepartmentId = @DepartmentId,
                    LocationId = @LocationId,
                    CategoryId = @CategoryId,
                    PriorityId = @PriorityId,
                    StatusId = @StatusId,
                    TicketTypeId = @TicketTypeId,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteAsync(sql, ticket),
                "Failed to update ticket.");
        }

        public Task<int> DeleteAsync(int ticketId)
        {
            const string sql = @"DELETE FROM Tickets WHERE Id = @TicketId";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteAsync(sql, new { TicketId = ticketId }),
                "Failed to delete ticket.");
        }

        public Task<IEnumerable<TicketLog>> GetLogsAsync(int ticketId)
        {
            const string sql = @"SELECT * FROM TicketLogs WHERE TicketId = @TicketId ORDER BY CreatedAt ASC";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryAsync<TicketLog>(sql, new { TicketId = ticketId }),
                "Failed to retrieve ticket logs.");
        }

        public Task<int> AddLogAsync(TicketLog log)
        {
            const string sql = @"
                INSERT INTO TicketLogs (TicketId, Comment, CreatedBy, CreatedByRole, CreatedAt)
                VALUES (@TicketId, @Comment, @CreatedBy, @CreatedByRole, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteScalarAsync<int>(sql, log),
                "Failed to add ticket log.");
        }

        public Task<int> AssignStaffAsync(int ticketId, int staffId)
        {
            const string sql = @"UPDATE Tickets SET StaffId = @StaffId, UpdatedAt = GETDATE() WHERE Id = @TicketId";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteAsync(sql, new { TicketId = ticketId, StaffId = staffId }),
                "Failed to assign staff.");
        }

        public Task<int> UpdateStatusAsync(int ticketId, int statusId)
        {
            const string sql = @"UPDATE Tickets SET StatusId = @StatusId, UpdatedAt = GETDATE() WHERE Id = @TicketId";
            return  RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteAsync(sql, new { TicketId = ticketId, StatusId = statusId }),
                "Failed to update ticket status.");
        }

        public Task<IEnumerable<Ticket>> GetByStatusAsync(int statusId)
        {
            const string sql = @"SELECT * FROM Tickets WHERE StatusId = @StatusId ORDER BY CreatedAt DESC";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryAsync<Ticket>(sql, new { StatusId = statusId }),
                "Failed to retrieve tickets by status.");
        }

        public Task<IEnumerable<Ticket>> GetByStaffAsync(int staffId)
        {
            const string sql = @"SELECT * FROM Tickets WHERE StaffId = @StaffId ORDER BY CreatedAt DESC";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryAsync<Ticket>(sql, new { StaffId = staffId }),
                "Failed to retrieve tickets by staff.");
        }

        public Task<IEnumerable<Ticket>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT * FROM Tickets 
                WHERE CreatedAt BETWEEN @StartDate AND @EndDate
                ORDER BY CreatedAt DESC";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryAsync<Ticket>(sql, new { StartDate = startDate, EndDate = endDate }),
                "Failed to retrieve tickets by date range.");
        }

        public Task<int> AddCommentAsync(Comment comment)
        {
            const string sql = @"
                INSERT INTO TicketComments (TicketId, Text, CreatedByUserId, CreatedAt)
                VALUES (@TicketId, @Text, @CreatedByUserId, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteScalarAsync<int>(sql, comment),
                "Failed to add comment.");
        }

        public Task<IEnumerable<Comment>> GetCommentsAsync(int ticketId)
        {
            const string sql = @"SELECT * FROM TicketComments WHERE TicketId = @TicketId ORDER BY CreatedAt ASC";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryAsync<Comment>(sql, new { TicketId = ticketId }),
                "Failed to retrieve comments.");
        }

        public Task<Dictionary<int, int>> GetTicketCountGroupedByStatusAsync()
        {
            const string sql = @"SELECT StatusId, COUNT(*) AS Count FROM Tickets GROUP BY StatusId";
            return RepositoryHelper.ExecuteWithHandlingAsync(async () =>
            {
                var result = await _dbConnection.QueryAsync<(int StatusId, int Count)>(sql);
                return result.ToDictionary(r => r.StatusId, r => r.Count);
            }, "Failed to count tickets grouped by status.");
        }

        public Task<IEnumerable<Ticket>> GetByPriorityAsync(int priorityId)
        {
            const string sql = @"SELECT * FROM Tickets WHERE PriorityId = @PriorityId ORDER BY CreatedAt DESC";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.QueryAsync<Ticket>(sql, new { PriorityId = priorityId }),
                "Failed to retrieve tickets by priority.");
        }

        public Task<int> BulkUpdateStatusAsync(IEnumerable<int> ticketIds, int statusId)
        {
            const string sql = @"
                UPDATE Tickets
                SET StatusId = @StatusId, UpdatedAt = GETDATE()
                WHERE Id IN @TicketIds";
            return RepositoryHelper.ExecuteWithHandlingAsync(
                () => _dbConnection.ExecuteAsync(sql, new { StatusId = statusId, TicketIds = ticketIds }),
                "Failed to bulk update ticket statuses.");
        }

        public async Task<(IEnumerable<Ticket>, int totalCount)> GetFilteredTicketsPagedAsync(TicketFilterDto filter, int page, int pageSize)
        {
            try
            {
                var sql = @"SELECT * FROM Tickets WHERE 1=1";
                var countSql = @"SELECT COUNT(*) FROM Tickets WHERE 1=1";
                var parameters = new DynamicParameters();
                var whereClause = "";

                if (filter.StatusId.HasValue) { whereClause += " AND StatusId = @StatusId"; parameters.Add("StatusId", filter.StatusId.Value); }
                if (filter.StaffId.HasValue) { whereClause += " AND StaffId = @StaffId"; parameters.Add("StaffId", filter.StaffId.Value); }
                if (filter.PriorityId.HasValue) { whereClause += " AND PriorityId = @PriorityId"; parameters.Add("PriorityId", filter.PriorityId.Value); }
                if (filter.DepartmentId.HasValue) { whereClause += " AND DepartmentId = @DepartmentId"; parameters.Add("DepartmentId", filter.DepartmentId.Value); }
                if (filter.UserId.HasValue) { whereClause += " AND UserId = @UserId"; parameters.Add("UserId", filter.UserId.Value); }
                if (filter.LocationId.HasValue) { whereClause += " AND LocationId = @LocationId"; parameters.Add("LocationId", filter.LocationId.Value); }
                if (filter.CategoryId.HasValue) { whereClause += " AND CategoryId = @CategoryId"; parameters.Add("CategoryId", filter.CategoryId.Value); }
                if (filter.CreatedAfter.HasValue) { whereClause += " AND CreatedAt >= @CreatedAfter"; parameters.Add("CreatedAfter", filter.CreatedAfter.Value); }
                if (filter.CreatedBefore.HasValue) { whereClause += " AND CreatedAt <= @CreatedBefore"; parameters.Add("CreatedBefore", filter.CreatedBefore.Value); }
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    whereClause += " AND (Title LIKE @SearchTerm OR Description LIKE @SearchTerm)";
                    parameters.Add("SearchTerm", $"%{filter.SearchTerm}%");
                }

                sql += whereClause + " ORDER BY CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                countSql += whereClause;
                parameters.Add("Offset", (page - 1) * pageSize);
                parameters.Add("PageSize", pageSize);

                var tickets = await _dbConnection.QueryAsync<Ticket>(sql, parameters);
                var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, parameters);

                return (tickets, totalCount);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve paged filtered tickets.", ex);
            }
        } 
    }
}
