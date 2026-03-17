namespace GHR.LeaveManagement.Repositories
{   
    using System.Data;     
    using Microsoft.Data.SqlClient; 
    using Dapper;

    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.Entities;
    using GHR.LeaveManagement.Repositories.Interfaces;

    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly string _connectionString;

        public LeaveRequestRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); ;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<LeaveApplication>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveApplications";
            return await connection.QueryAsync<LeaveApplication>(sql);
        }

        public async Task<LeaveApplication?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveApplications WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<LeaveApplication>(sql, new { Id = id });
        }

        public async Task<IEnumerable<LeaveApplication>> GetByUserIdAsync(int userId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveApplications WHERE UserId = @UserId";
            return await connection.QueryAsync<LeaveApplication>(sql, new { UserId = userId });
        }

        public async Task<int> AddAsync(LeaveAppBindingModel request)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO LeaveApplications (
                    UserId, LeaveTypeId, FullName, Department, Email, PhoneNumber,
                    StartDate, EndDate, TotalDays, Reason, Status, ApproverId, DecisionDate, RequestedAt
                )
                VALUES (
                    @UserId, @LeaveTypeId, @FullName, @Department, @Email, @PhoneNumber,
                    @StartDate, @EndDate, @TotalDays, @Reason, @Status, @ApproverId, @DecisionDate, @RequestedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            request.Status ??= "Pending";
            request.RequestedAt = DateTime.UtcNow;

            return await connection.ExecuteScalarAsync<int>(sql, request);
        }

        public async Task UpdateAsync(LeaveApplication request)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE LeaveApplications
                SET
                    UserId = @UserId,
                    FullName = @FullName,
                    Department = @Department,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    LeaveTypeId = @LeaveTypeId,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    TotalDays = @TotalDays,
                    Reason = @Reason,
                    Status = @Status,
                    ApproverId = @ApproverId,
                    DecisionDate = @DecisionDate
                WHERE Id = @Id
            ";
            await connection.ExecuteAsync(sql, request);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM LeaveApplications WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<int>> GetLeaveApplicationsIdsAsync(string status)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT UserId FROM LeaveApplications WHERE Status = @Status";
            return await connection.QueryAsync<int>(sql, new { Status = status });
        }
    }
}
