namespace GHR.DutyManagement.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;
    
    using Dapper;
    using GHR.DutyManagement.DTOs;
    using GHR.DutyManagement.Entities;
 

    public interface IDutyRepository
    {
        Task<IEnumerable<Duty>> GetAllDutiesAsync();
        Task<Duty> GetDutyByIdAsync(int id);
        Task<int> CreateDutyAsync(DutyDTO duty);
        Task<bool> UpdateDutyAsync(Duty duty);
        Task<bool> DeleteDutyAsync(int id);
        Task<IEnumerable<Shift>> GetAllShiftsAsync();
        Task<IEnumerable<PeriodType>> GetAllPeriodTypesAsync();
        Task<IEnumerable<DutyAssignment>> GetDutyAssignmentsAsync(int dutyId);
        Task<IEnumerable<EmployeeIdManagerIdDTO>> GetAvailableStaffAsync(string facility);
        Task<IEnumerable<Duty>> GetByFacilityAndStatusAsync(string facility, string status);
        Task<int> AssignDutyAsync(DutyAssignmentDTO dutyAssignment);
    }

    public class DutyRepository : IDutyRepository
    {
        private readonly string _connectionString;

        public DutyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); ;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<int> AssignDutyAsync(DutyAssignmentDTO dutyAssignment)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO DutyAssignments (EmployeeId, PeriodTypeId, DutyId, ShiftId, AssignmentDate)
                VALUES (@EmployeeId, @PeriodTypeId, @DutyId, @ShiftId, @AssignmentDate);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await connection.ExecuteScalarAsync<int>(sql, dutyAssignment);
        }

        public async Task<int> CreateDutyAsync(DutyDTO duty)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO Duties
                    (Title, Description, AssignedToUserId, AssignedByUserId, RoleRequired, Facility, Status, Priority, DueDate)
                OUTPUT INSERTED.Id
                VALUES
                    (@Title, @Description, @AssignedToUserId, @AssignedByUserId, @RoleRequired, @Facility, @Status, @Priority, @DueDate);";
            return await connection.ExecuteScalarAsync<int>(sql, duty);
        }

        public async Task<bool> DeleteDutyAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Duties WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Duty>> GetAllDutiesAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Duties";
            return await connection.QueryAsync<Duty>(sql);
        }

        public async Task<IEnumerable<PeriodType>> GetAllPeriodTypesAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM PeriodType";
            return await connection.QueryAsync<PeriodType>(sql);
        }

        public async Task<IEnumerable<Shift>> GetAllShiftsAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Shifts";
            return await connection.QueryAsync<Shift>(sql);
        }

        public async Task<IEnumerable<DutyAssignment>> GetDutyAssignmentsAsync(int dutyId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM DutyAssignments WHERE DutyId = @DutyId";
            return await connection.QueryAsync<DutyAssignment>(sql, new { DutyId = dutyId });
        }

        public async Task<IEnumerable<EmployeeIdManagerIdDTO>> GetAvailableStaffAsync(string facility)
        {
            using var connection = CreateConnection(); 
            var sql = @"
                SELECT 
                    AssignedToUserId AS EmployeeId,
                    AssignedByUserId AS ManagerId,
                    Facility
                FROM Duties
                WHERE Status = 'Completed' AND Facility = @Facility";
            return await connection.QueryAsync<EmployeeIdManagerIdDTO>(sql, new { Facility = facility });
        }

        public async Task<IEnumerable<Duty>> GetByFacilityAndStatusAsync(string facility, string status)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Duties WHERE Facility = @Facility AND Status = @Status";
            return await connection.QueryAsync<Duty>(sql, new { Facility = facility, Status = status });
        }

        public async Task<Duty> GetDutyByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Duties WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Duty>(sql, new { Id = id });
        }

        public async Task<bool> UpdateDutyAsync(Duty duty)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE Duties SET
                    Title = @Title,
                    Description = @Description,
                    AssignedToUserId = @AssignedToUserId,
                    AssignedByUserId = @AssignedByUserId,
                    RoleRequired = @RoleRequired,
                    Facility = @Facility,
                    Status = @Status,
                    Priority = @Priority,
                    DueDate = @DueDate,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, duty);
            return rowsAffected > 0;
        }
    }
}
