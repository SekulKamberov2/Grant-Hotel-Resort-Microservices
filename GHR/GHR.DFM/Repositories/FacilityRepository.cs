namespace GHR.DFM.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;
    using Dapper;
    using GHR.DFM.Entities; 

    public interface IFacilityRepository
    {
        Task<IEnumerable<Facility>> GetAllAsync();
        Task<Facility?> GetByIdAsync(int id);
        Task<int> CreateAsync(Facility facility);
        Task<bool> UpdateAsync(Facility facility);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<string>> GetFacilityTypesAsync();
        Task<IEnumerable<string>> GetFacilityStatusesAsync();
        Task<bool> CreateFacilityScheduleAsync(FacilitySchedule schedule);
        Task<bool> UpdateFacilityStatusAsync(int id, string status); 
        Task<IEnumerable<Facility>> GetAvailableFacilitiesAsync();
        Task<IEnumerable<FacilitySchedule>> GetFacilityScheduleAsync(int facilityId);
        Task<bool> UpdateFacilityScheduleAsync(int facilityId, IEnumerable<FacilitySchedule> schedules); 
        Task<IEnumerable<Facility>> GetNearbyFacilitiesAsync(string location);
        Task<IEnumerable<FacilityServiceItem>> GetFacilityServicesAsync(int facilityId);
        Task<int> AddFacilityServiceAsync(FacilityServiceItem service); 
        Task<bool> DeleteFacilityServiceAsync(int facilityId, int serviceId);
        Task<int> CreateReservationAsync(FacilityReservation reservation);
        Task<IEnumerable<FacilityReservation>> GetReservationsByFacilityAsync(int facilityId); 
        Task<bool> DeleteReservationAsync(int facilityId, int reservationId);
        Task<int> ReportIssueAsync(FacilityIssue issue);
        Task<IEnumerable<FacilityIssue>> GetOpenIssuesAsync(int facilityId); 
        Task<bool> AssignMaintenanceAsync(int facilityId, int issueId, string assignedTo);
        Task<IEnumerable<FacilityReservation>> GetUsageHistoryAsync(int facilityId);
        Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(int facilityId, DateTime date);
         
    }

    public class FacilityRepository : IFacilityRepository
    {
        private readonly string _connectionString;

        public FacilityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Facility>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Facilities";
            return await connection.QueryAsync<Facility>(sql);
        }

        public async Task<Facility?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Facilities WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Facility>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Facility facility)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO Facilities (Name, Description, Location, Department, Status, CreatedAt)
                VALUES (@Name, @Description, @Location, @Department, @Status, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, facility);
        }

        public async Task<bool> UpdateAsync(Facility facility)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE Facilities SET 
                    Name = @Name,
                    Description = @Description,
                    Location = @Location,
                    Department = @Department,
                    Status = @Status,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, facility);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Facilities WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<string>> GetFacilityTypesAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT DISTINCT Department FROM Facilities WHERE Department IS NOT NULL";
            return await connection.QueryAsync<string>(sql);
        }

        public async Task<IEnumerable<string>> GetFacilityStatusesAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT DISTINCT Status FROM Facilities WHERE Status IS NOT NULL";
            return await connection.QueryAsync<string>(sql);
        }

        public async Task<bool> UpdateFacilityStatusAsync(int id, string status)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Facilities SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Status = status, UpdatedAt = DateTime.UtcNow, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> CreateFacilityScheduleAsync(FacilitySchedule schedule)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO FacilitySchedules (FacilityId, DayOfWeek, OpenTime, CloseTime, IsMaintenance)
                VALUES (@FacilityId, @DayOfWeek, @OpenTime, @CloseTime, @IsMaintenance)";
            var rowsAffected = await connection.ExecuteAsync(sql, schedule);
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Facility>> GetAvailableFacilitiesAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Facilities WHERE Status = @Status";
            return await connection.QueryAsync<Facility>(sql, new { Status = "Active" });
        }

        public async Task<IEnumerable<FacilitySchedule>> GetFacilityScheduleAsync(int facilityId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM FacilitySchedules WHERE FacilityId = @FacilityId ORDER BY DayOfWeek, OpenTime";
            return await connection.QueryAsync<FacilitySchedule>(sql, new { FacilityId = facilityId });
        }

        public async Task<bool> UpdateFacilityScheduleAsync(int facilityId, IEnumerable<FacilitySchedule> schedules)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var deleteSql = "DELETE FROM FacilitySchedules WHERE FacilityId = @FacilityId";
                await connection.ExecuteAsync(deleteSql, new { FacilityId = facilityId }, transaction);

                var insertSql = @"
                    INSERT INTO FacilitySchedules (FacilityId, DayOfWeek, OpenTime, CloseTime, IsMaintenance)
                    VALUES (@FacilityId, @DayOfWeek, @OpenTime, @CloseTime, @IsMaintenance)";

                foreach (var sched in schedules)
                {
                    await connection.ExecuteAsync(insertSql, new
                    {
                        FacilityId = facilityId,
                        DayOfWeek = sched.DayOfWeek,
                        OpenTime = sched.OpenTime,
                        CloseTime = sched.CloseTime,
                        IsMaintenance = sched.IsMaintenance
                    }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Facility>> GetNearbyFacilitiesAsync(string location)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Facilities WHERE Location LIKE @Pattern";
            return await connection.QueryAsync<Facility>(sql, new { Pattern = $"%{location}%" });
        }

        public async Task<IEnumerable<FacilityServiceItem>> GetFacilityServicesAsync(int facilityId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM FacilityServices WHERE FacilityId = @FacilityId"; // Fixed column name
            return await connection.QueryAsync<FacilityServiceItem>(sql, new { FacilityId = facilityId });
        }

        public async Task<int> AddFacilityServiceAsync(FacilityServiceItem service)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO FacilityServices (FacilityId, Name, Description, Price, DurationMinutes)
                VALUES (@FacilityId, @Name, @Description, @Price, @DurationMinutes);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, service);
        }

        public async Task<bool> DeleteFacilityServiceAsync(int facilityId, int serviceId)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM FacilityServices WHERE FacilityId = @FacilityId AND ServiceId = @ServiceId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { FacilityId = facilityId, ServiceId = serviceId });
            return rowsAffected > 0;
        }

        public async Task<int> CreateReservationAsync(FacilityReservation reservation)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO FacilityReservations (FacilityId, ReservedBy, ReservationDate, StartTime, EndTime, Purpose, CreatedAt)
                VALUES (@FacilityId, @ReservedBy, @ReservationDate, @StartTime, @EndTime, @Purpose, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await connection.ExecuteScalarAsync<int>(sql, reservation);
        }

        public async Task<IEnumerable<FacilityReservation>> GetReservationsByFacilityAsync(int facilityId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM FacilityReservations WHERE FacilityId = @FacilityId ORDER BY ReservationDate, StartTime";
            return await connection.QueryAsync<FacilityReservation>(sql, new { FacilityId = facilityId });
        }

        public async Task<bool> DeleteReservationAsync(int facilityId, int reservationId)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM FacilityReservations WHERE FacilityId = @FacilityId AND ReservationId = @ReservationId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { FacilityId = facilityId, ReservationId = reservationId });
            return rowsAffected > 0;
        }

        public async Task<int> ReportIssueAsync(FacilityIssue issue)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO FacilityIssues (FacilityId, ReportedBy, Description, Status, ReportedAt)
                VALUES (@FacilityId, @ReportedBy, @Description, 'Open', GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await connection.ExecuteScalarAsync<int>(sql, issue);
        }

        public async Task<IEnumerable<FacilityIssue>> GetOpenIssuesAsync(int facilityId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM FacilityIssues WHERE FacilityId = @FacilityId AND Status = 'Open' ORDER BY ReportedAt DESC";
            return await connection.QueryAsync<FacilityIssue>(sql, new { FacilityId = facilityId });
        }

        public async Task<bool> AssignMaintenanceAsync(int facilityId, int issueId, string assignedTo)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE FacilityIssues
                SET AssignedTo = @AssignedTo, AssignedAt = GETDATE()
                WHERE FacilityId = @FacilityId AND IssueId = @IssueId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { FacilityId = facilityId, IssueId = issueId, AssignedTo = assignedTo });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<FacilityReservation>> GetUsageHistoryAsync(int facilityId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM FacilityReservations WHERE FacilityId = @FacilityId ORDER BY ReservationDate DESC";
            return await connection.QueryAsync<FacilityReservation>(sql, new { FacilityId = facilityId });
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(int facilityId, DateTime date)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT StartTime, EndTime FROM FacilityReservations
                WHERE FacilityId = @FacilityId AND ReservationDate = @Date";
            var reserved = await connection.QueryAsync<(TimeSpan StartTime, TimeSpan EndTime)>(
                sql, new { FacilityId = facilityId, Date = date });

            var allSlots = Enumerable.Range(8, 12) // 08:00–20:00
                .Select(h => new TimeSpan(h, 0, 0)).ToList();

            var reservedSlots = new HashSet<TimeSpan>(reserved.Select(r => r.StartTime));
            return allSlots.Where(t => !reservedSlots.Contains(t));
        }
    }
}
