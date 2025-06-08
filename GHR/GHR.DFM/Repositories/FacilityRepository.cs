namespace GHR.DFM.Repositories
{
    using Dapper;
    using GHR.DFM.Entities;
    using GHR.SharedKernel.Helpers;
    using Microsoft.Data.SqlClient; 
    using System.Data;
    using System.Data.Common;

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
        private readonly IDbConnection _db; 
        public FacilityRepository(IConfiguration config)  =>
            _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));

        public async Task<IEnumerable<Facility>> GetAllAsync()
        {
            var sql = "SELECT * FROM Facilities"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                 () => _db.QueryAsync<Facility>(sql),
                 "Failed to select Facilities.");
        }

        public async Task<Facility?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Facilities WHERE Id = @Id"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                 () => _db.QueryFirstOrDefaultAsync<Facility>(sql, new { Id = id }),
                 "Failed to get by Id.");
        }

        public async Task<int> CreateAsync(Facility facility)
        {
            var sql = @"
            INSERT INTO Facilities (Name, Description, Location, Department, Status, CreatedAt)
            VALUES (@Name, @Description, @Location, @Department, @Status, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int);"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                () => _db.ExecuteScalarAsync<int>(sql, facility),
                "Failed to create."); 
        }

        public async Task<bool> UpdateAsync(Facility facility)
        {
            var sql = @"
            UPDATE Facilities SET 
                Name = @Name,
                Description = @Description,
                Location = @Location,
                Department = @Department,
                Status = @Status,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                 async () => await _db.ExecuteAsync(sql, facility) > 0,
            "Failed to create.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Facilities WHERE Id = @Id"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteAsync(sql, new { Id = id }) > 0,
            "Failed to create.");
        }

        public async Task<IEnumerable<string>> GetFacilityTypesAsync()
        { 
            var sql = "SELECT DISTINCT Department FROM Facilities WHERE Department IS NOT NULL";
            return await _db.QueryAsync<string>(sql);
        }

        public async Task<IEnumerable<string>> GetFacilityStatusesAsync()
        {
            var sql = "SELECT DISTINCT Status FROM Facilities WHERE Status IS NOT NULL";
            return await _db.QueryAsync<string>(sql);
        }

        public async Task<bool> UpdateFacilityStatusAsync(int id, string status)
        {
            var sql = "UPDATE Facilities SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @Id"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteAsync(sql, new { Status = status, UpdatedAt = DateTime.UtcNow, Id = id }) > 0,
            "Failed to create.");
        }

        public async Task<bool> CreateFacilityScheduleAsync(FacilitySchedule schedule) 
        {
            var sql = @"
            INSERT INTO FacilitySchedules (FacilityId, DayOfWeek, OpenTime, CloseTime, IsMaintenance)
            VALUES (@FacilityId, @DayOfWeek, @OpenTime, @CloseTime, @IsMaintenance);
            SELECT CAST(SCOPE_IDENTITY() as int);";
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteScalarAsync<int>(sql, schedule) > 0,
            "Failed to create.");
        }

        public async Task<IEnumerable<Facility>> GetAvailableFacilitiesAsync()
        { 
            var sql = "SELECT * FROM Facilities WHERE Status = @Status";
            return await _db.QueryAsync<Facility>(sql, new { Status = "Active" }); //Active status is open
        }

        public async Task<IEnumerable<FacilitySchedule>> GetFacilityScheduleAsync(int Id)
        {
            var sql = "SELECT * FROM FacilitySchedules WHERE FacilityId = @FacilityId ORDER BY DayOfWeek, OpenTime";
            return await _db.QueryAsync<FacilitySchedule>(sql, new { FacilityId = Id });
        }

        public async Task<bool> UpdateFacilityScheduleAsync(int FacilityId, IEnumerable<FacilitySchedule> schedules)
        {
            if (_db.State != ConnectionState.Open)
                await ((DbConnection)_db).OpenAsync();

            using var transaction = _db.BeginTransaction();

            try
            {
                var deleteSql = "DELETE FROM FacilitySchedules WHERE FacilityId = @FacilityId";
                await _db.ExecuteAsync(deleteSql, new { FacilityId = FacilityId }, transaction);

                var insertSql = @"
                    INSERT INTO FacilitySchedules (FacilityId, DayOfWeek, OpenTime, CloseTime, IsMaintenance)
                    VALUES (@FacilityId, @DayOfWeek, @OpenTime, @CloseTime, @IsMaintenance)";

                foreach (var sched in schedules)
                {
                    await _db.ExecuteAsync(insertSql, new
                    {
                        FacilityId = FacilityId,
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
            var sql = "SELECT * FROM Facilities WHERE Location LIKE @Pattern";
            return await _db.QueryAsync<Facility>(sql, new { Pattern = $"%{location}%" });
        }

        public async Task<IEnumerable<FacilityServiceItem>> GetFacilityServicesAsync(int Id)
        {
            var sql = "SELECT * FROM FacilityServices WHERE ServiceId = @ServiceId";
            return await _db.QueryAsync<FacilityServiceItem>(sql, new { ServiceId = Id });
        }

        public async Task<int> AddFacilityServiceAsync(FacilityServiceItem service)
        {
            var sql = @"
            INSERT INTO FacilityServices (Id, Name, Description, Price, DurationMinutes)
            VALUES (@FacilityId, @Name, @Description, @Price, @DurationMinutes);
            SELECT CAST(SCOPE_IDENTITY() as int);";
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteScalarAsync<int>(sql, service),
            "Failed to add.");
        }
        public async Task<bool> DeleteFacilityServiceAsync(int Id, int serviceId)
        {
            var sql = "DELETE FROM FacilityServices WHERE ServiceId = @ServiceId AND Id = @Id"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteAsync(sql, new { Id = Id, ServiceId = serviceId }) > 0,
            "Failed to delete.");
        }

        public async Task<int> CreateReservationAsync(FacilityReservation reservation)
        {
            var sql = @"
            INSERT INTO FacilityReservations (Id, ReservedBy, ReservationDate, StartTime, EndTime, Purpose, CreatedAt)
            VALUES (@FacilityId, @ReservedBy, @ReservationDate, @StartTime, @EndTime, @Purpose, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteScalarAsync<int>(sql, reservation),
            "Failed to Create.");
        }

        public async Task<IEnumerable<FacilityReservation>> GetReservationsByFacilityAsync(int Id)
        {
            var sql = "SELECT * FROM FacilityReservations WHERE Id = @Id ORDER BY ReservationDate, StartTime";
            return await _db.QueryAsync<FacilityReservation>(sql, new { Id = Id });
        }   

        public async Task<bool> DeleteReservationAsync(int Id, int reservationId)
        {
            var sql = "DELETE FROM FacilityReservations WHERE Id = @Id AND ReservationId = @ReservationId";  
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteAsync(sql, new { Id = Id, ReservationId = reservationId }) > 0,
            "Failed to delete.");
        }

        public async Task<int> ReportIssueAsync(FacilityIssue issue)
        {
            var sql = @"
            INSERT INTO FacilityIssues (Id, ReportedBy, Description, Status, ReportedAt)
            VALUES (@FacilityId, @ReportedBy, @Description, 'Open', GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteScalarAsync<int>(sql, issue),
            "Failed to delete.");
        }

        public async Task<IEnumerable<FacilityIssue>> GetOpenIssuesAsync(int Id)
        {
            var sql = "SELECT * FROM FacilityIssues WHERE Id = @Id AND Status = 'Open' ORDER BY ReportedAt DESC";
            return await _db.QueryAsync<FacilityIssue>(sql, new { Id = Id });
        }

        public async Task<bool> AssignMaintenanceAsync(int Id, int issueId, string assignedTo)
        {
            var sql = @"
			UPDATE FacilityIssues
			SET AssignedTo = @AssignedTo, AssignedAt = GETDATE()
			WHERE Id = @Id AND IssueId = @IssueId";  
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                async () => await _db.ExecuteAsync(sql, new { Id = Id, IssueId = issueId, AssignedTo = assignedTo }) > 0,
            "Failed to delete.");
        }

        public async Task<IEnumerable<FacilityReservation>> GetUsageHistoryAsync(int Id)
        {
            var sql = "SELECT * FROM FacilityReservations WHERE Id = @Id ORDER BY ReservationDate DESC";
            return await _db.QueryAsync<FacilityReservation>(sql, new { Id = Id });
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(int Id, DateTime date)
        {
            var sql = @"
			SELECT StartTime, EndTime FROM FacilityReservations
			WHERE Id = @Id AND ReservationDate = @Date";

            var reserved = await _db.QueryAsync<(TimeSpan StartTime, TimeSpan EndTime)>(
                sql, new { Id = Id, Date = date });

            var allSlots = Enumerable.Range(8, 12) // 08:00–20:00
                .Select(h => new TimeSpan(h, 0, 0)).ToList();

            var reservedSlots = new HashSet<TimeSpan>(
                reserved.Select(r => r.StartTime));

            var available = allSlots.Where(t => !reservedSlots.Contains(t));
            return available;
        } 
    }
}
