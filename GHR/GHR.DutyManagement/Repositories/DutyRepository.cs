namespace GHR.DutyManagement.Repositories
{
    using Dapper;
    using GHR.DutyManagement.DTOs;
    using GHR.DutyManagement.Entities;
    using Microsoft.Data.SqlClient;
    using System.Data;

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
        private readonly IDbConnection _connection;
        public DutyRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<int> AssignDutyAsync(DutyAssignmentDTO dutyAssignment)
        {
            var sql = "INSERT INTO DutyAssignments (EmployeeId, PeriodTypeId, DutyId, ShiftId, AssignmentDate) " +
                      "VALUES (@EmployeeId, @PeriodTypeId, @DutyId, @ShiftId, @AssignmentDate); " +
                      "SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _connection.ExecuteScalarAsync<int>(sql, dutyAssignment);
        }

        //public async Task<int> CreateDutyAsync(DutyDTO duty)
        //{
        //    var sql = "INSERT INTO Duties (Title, Description, AssignedToUserId, AssignedByUserId, RoleRequired, Facility, Status, Priority, DueDate) " +
        //              "VALUES (@Title, @Description, @AssignedToUserId, @AssignedByUserId, @RoleRequired, @Facility, @Status, @Priority, @DueDate); " +
        //              "SELECT CAST(SCOPE_IDENTITY() AS INT);";
        //    return await _connection.ExecuteScalarAsync<int>(sql, duty);
        //}
        public async Task<int> CreateDutyAsync(DutyDTO duty)
        {
            var sql = @"
                INSERT INTO Duties 
                    (Title, Description, AssignedToUserId, AssignedByUserId, RoleRequired, Facility, Status, Priority, DueDate)
                OUTPUT INSERTED.Id
                VALUES
                     (@Title, @Description, @AssignedToUserId, @AssignedByUserId, @RoleRequired, @Facility, @Status, @Priority, @DueDate);";

            var createdDutyId = await _connection.ExecuteScalarAsync<int>(sql, duty); 
            return createdDutyId;
        }


        public async Task<bool> DeleteDutyAsync(int id)
        {
            var sql = "DELETE FROM Duties WHERE Id = @Id";
            var result = await _connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<IEnumerable<Duty>> GetAllDutiesAsync()
        {
            var sql = "SELECT * FROM Duties";
            return await _connection.QueryAsync<Duty>(sql);
        }

        public async Task<IEnumerable<PeriodType>> GetAllPeriodTypesAsync()
        {
            var sql = "SELECT * FROM PeriodType";
            return await _connection.QueryAsync<PeriodType>(sql);
        }

        public async Task<IEnumerable<Shift>> GetAllShiftsAsync()
        {
            var sql = "SELECT * FROM Shifts";
            return await _connection.QueryAsync<Shift>(sql);
        }

        public async Task<IEnumerable<DutyAssignment>> GetDutyAssignmentsAsync(int dutyId)
        {
            var sql = "SELECT * FROM DutyAssignments WHERE DutyId = @DutyId";
            return await _connection.QueryAsync<DutyAssignment>(sql, new { DutyId = dutyId });
        }
        public async Task<IEnumerable<EmployeeIdManagerIdDTO>> GetAvailableStaffAsync(string facility)
        {
            var sql = @"SELECT AssignedToUserId, AssignedByUserId, Facility 
                        FROM Duties  WHERE Status = 'Completed' AND Facility = @Facility"; 
            return await _connection.QueryAsync<EmployeeIdManagerIdDTO>(sql, new { Facility = facility });
        }

        public async Task<IEnumerable<Duty>> GetByFacilityAndStatusAsync(string facility, string status)
        {
            var sql = @"SELECT * FROM Duties WHERE Facility = @Facility AND Status = @Status;";
            return await _connection.QueryAsync<Duty>(sql, new { Facility = facility, Status = status });  
        }

        public async Task<Duty> GetDutyByIdAsync(int id)
        {
            var sql = "SELECT * FROM Duties WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<Duty>(sql, new { Id = id });
        }

        public async Task<bool> UpdateDutyAsync(Duty duty)
        {
            var sql = "UPDATE Duties SET Title = @Title, Description = @Description, AssignedToUserId = @AssignedToUserId, " +
                      "AssignedByUserId = @AssignedByUserId, RoleRequired = @RoleRequired, Facility = @Facility, Status = @Status, " +
                      "Priority = @Priority, DueDate = @DueDate, UpdatedAt = GETDATE() WHERE Id = @Id";
            var result = await _connection.ExecuteAsync(sql, duty);
            return result > 0;
        }
    }
}
