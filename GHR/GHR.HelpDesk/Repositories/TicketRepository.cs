namespace GHR.HelpDesk.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;
 
    using Dapper;
    using GHR.HelpDesk.DTOs;
    using GHR.HelpDesk.Entities;  
 
    public interface ITicketRepository
    {
        Task<Ticket> GetByIdAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<IEnumerable<Ticket>> GetAllUserTicketsAsync(int userId);
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
        private readonly string _connectionString;

        public TicketRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Ticket?> GetByIdAsync(int ticketId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Tickets WHERE Id = @TicketId";
            return await connection.QueryFirstOrDefaultAsync<Ticket>(sql, new { TicketId = ticketId });
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Tickets ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Ticket>(sql);
        }

        public async Task<IEnumerable<Ticket>> GetAllUserTicketsAsync(int userId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Tickets WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Ticket>(sql, new { UserId = userId });
        }

        public async Task<int> CreateAsync(Ticket ticket)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO Tickets (Title, Description, UserId, StaffId, DepartmentId, LocationId,
                                     CategoryId, PriorityId, StatusId, TicketTypeId, CreatedAt)
                VALUES (@Title, @Description, @UserId, @StaffId, @DepartmentId, @LocationId,
                        @CategoryId, @PriorityId, @StatusId, @TicketTypeId, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, ticket);
        }

        public async Task<int> UpdateAsync(Ticket ticket)
        {
            using var connection = CreateConnection();
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
            return await connection.ExecuteAsync(sql, ticket);
        }

        public async Task<int> DeleteAsync(int ticketId)
        {
            using var connection = CreateConnection();
            const string sql = @"DELETE FROM Tickets WHERE Id = @TicketId";
            return await connection.ExecuteAsync(sql, new { TicketId = ticketId });
        }

        public async Task<IEnumerable<TicketLog>> GetLogsAsync(int ticketId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM TicketLogs WHERE TicketId = @TicketId ORDER BY CreatedAt ASC";
            return await connection.QueryAsync<TicketLog>(sql, new { TicketId = ticketId });
        }

        public async Task<int> AddLogAsync(TicketLog log)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO TicketLogs (TicketId, Comment, CreatedBy, CreatedByRole, CreatedAt)
                VALUES (@TicketId, @Comment, @CreatedBy, @CreatedByRole, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, log);
        }

        public async Task<int> AssignStaffAsync(int ticketId, int staffId)
        {
            using var connection = CreateConnection();
            const string sql = @"UPDATE Tickets SET StaffId = @StaffId, UpdatedAt = GETDATE() WHERE Id = @TicketId";
            return await connection.ExecuteAsync(sql, new { TicketId = ticketId, StaffId = staffId });
        }

        public async Task<int> UpdateStatusAsync(int ticketId, int statusId)
        {
            using var connection = CreateConnection();
            const string sql = @"UPDATE Tickets SET StatusId = @StatusId, UpdatedAt = GETDATE() WHERE Id = @TicketId";
            return await connection.ExecuteAsync(sql, new { TicketId = ticketId, StatusId = statusId });
        }

        public async Task<IEnumerable<Ticket>> GetByStatusAsync(int statusId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Tickets WHERE StatusId = @StatusId ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Ticket>(sql, new { StatusId = statusId });
        }

        public async Task<IEnumerable<Ticket>> GetByStaffAsync(int staffId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Tickets WHERE StaffId = @StaffId ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Ticket>(sql, new { StaffId = staffId });
        }

        public async Task<IEnumerable<Ticket>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = CreateConnection();
            const string sql = @"
                SELECT * FROM Tickets 
                WHERE CreatedAt BETWEEN @StartDate AND @EndDate
                ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Ticket>(sql, new { StartDate = startDate, EndDate = endDate });
        }

        public async Task<int> AddCommentAsync(Comment comment)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO TicketComments (TicketId, Text, CreatedByUserId, CreatedAt)
                VALUES (@TicketId, @Text, @CreatedByUserId, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, comment);
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(int ticketId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM TicketComments WHERE TicketId = @TicketId ORDER BY CreatedAt ASC";
            return await connection.QueryAsync<Comment>(sql, new { TicketId = ticketId });
        }

        public async Task<Dictionary<int, int>> GetTicketCountGroupedByStatusAsync()
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT StatusId, COUNT(*) AS Count FROM Tickets GROUP BY StatusId";
            var result = await connection.QueryAsync<(int StatusId, int Count)>(sql);
            return result.ToDictionary(r => r.StatusId, r => r.Count);
        }

        public async Task<IEnumerable<Ticket>> GetByPriorityAsync(int priorityId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Tickets WHERE PriorityId = @PriorityId ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<Ticket>(sql, new { PriorityId = priorityId });
        }

        public async Task<int> BulkUpdateStatusAsync(IEnumerable<int> ticketIds, int statusId)
        {
            using var connection = CreateConnection();
            const string sql = @"
                UPDATE Tickets
                SET StatusId = @StatusId, UpdatedAt = GETDATE()
                WHERE Id IN @TicketIds";
            return await connection.ExecuteAsync(sql, new { StatusId = statusId, TicketIds = ticketIds });
        }

        public async Task<(IEnumerable<Ticket>, int totalCount)> GetFilteredTicketsPagedAsync(TicketFilterDto filter, int page, int pageSize)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM Tickets WHERE 1=1";
            var countSql = @"SELECT COUNT(*) FROM Tickets WHERE 1=1";
            var parameters = new DynamicParameters();
            var whereClause = "";

            if (filter.StatusId.HasValue)
            {
                whereClause += " AND StatusId = @StatusId";
                parameters.Add("StatusId", filter.StatusId.Value);
            }
            if (filter.StaffId.HasValue)
            {
                whereClause += " AND StaffId = @StaffId";
                parameters.Add("StaffId", filter.StaffId.Value);
            }
            if (filter.PriorityId.HasValue)
            {
                whereClause += " AND PriorityId = @PriorityId";
                parameters.Add("PriorityId", filter.PriorityId.Value);
            }
            if (filter.DepartmentId.HasValue)
            {
                whereClause += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", filter.DepartmentId.Value);
            }
            if (filter.UserId.HasValue)
            {
                whereClause += " AND UserId = @UserId";
                parameters.Add("UserId", filter.UserId.Value);
            }
            if (filter.LocationId.HasValue)
            {
                whereClause += " AND LocationId = @LocationId";
                parameters.Add("LocationId", filter.LocationId.Value);
            }
            if (filter.CategoryId.HasValue)
            {
                whereClause += " AND CategoryId = @CategoryId";
                parameters.Add("CategoryId", filter.CategoryId.Value);
            }
            if (filter.CreatedAfter.HasValue)
            {
                whereClause += " AND CreatedAt >= @CreatedAfter";
                parameters.Add("CreatedAfter", filter.CreatedAfter.Value);
            }
            if (filter.CreatedBefore.HasValue)
            {
                whereClause += " AND CreatedAt <= @CreatedBefore";
                parameters.Add("CreatedBefore", filter.CreatedBefore.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                whereClause += " AND (Title LIKE @SearchTerm OR Description LIKE @SearchTerm)";
                parameters.Add("SearchTerm", $"%{filter.SearchTerm}%");
            }

            sql += whereClause + " ORDER BY CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            countSql += whereClause;
            parameters.Add("Offset", (page - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var tickets = await connection.QueryAsync<Ticket>(sql, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            return (tickets, totalCount);
        }
    }
}
