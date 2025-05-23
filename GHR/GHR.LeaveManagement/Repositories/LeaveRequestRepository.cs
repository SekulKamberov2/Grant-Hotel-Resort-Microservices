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
        private readonly IDbConnection _db;

        public LeaveRequestRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<LeaveApplication>> GetAllAsync()
        {
            var sql = "SELECT * FROM LeaveApplications";
            return await _db.QueryAsync<LeaveApplication>(sql);
        }

        public async Task<LeaveApplication?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM LeaveApplications WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<LeaveApplication>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(LeaveAppBindingModel request)
        {
            var sql = @"
            INSERT INTO LeaveApplications (
                UserId, LeaveTypeId, FullName, Department, Email, PhoneNumber, StartDate, EndDate, TotalDays, Reason, Status, ApproverId, DecisionDate, RequestedAt
            )
            VALUES (
                @UserId, @LeaveTypeId, @FullName, @Department, @Email, @PhoneNumber, @StartDate, @EndDate, @TotalDays, @Reason, @Status, @ApproverId, @DecisionDate, @RequestedAt
            );
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

            request.Status ??= "Pending";
            request.RequestedAt = DateTime.UtcNow;

            return await _db.ExecuteScalarAsync<int>(sql, request);
        }

        public async Task UpdateAsync(LeaveApplication request)
        {
            var sql = @"
            UPDATE LeaveApplications
            SET
                UserId = @UserId,
                FullName = @FullName, 
                Department = @Department, 
                Email = @Email,, 
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

            await _db.ExecuteAsync(sql, request);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM LeaveApplications WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }  
    } 
}
