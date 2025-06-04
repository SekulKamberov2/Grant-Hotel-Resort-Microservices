namespace GHR.RoomManagement.Repositories
{
    using System.Data;

    using Dapper;

    using GHR.RoomManagement.Entities;
    using GHR.SharedKernel.Helpers;
 
    public interface IBookingRepository
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<int> CreateReservationAsync(Reservation reservation);
        Task<bool> UpdateReservationAsync(Reservation reservation);
        Task<bool> DeleteReservationAsync(int id); 
        Task<IEnumerable<RoomRate>> GetAllRoomRatesAsync();
        Task<RoomRate?> GetRoomRateByIdAsync(int id);
        Task<int> CreateRoomRateAsync(RoomRate rate);
        Task<bool> UpdateRoomRateAsync(RoomRate rate);
        Task<bool> DeleteRoomRateAsync(int id); 
        Task<bool> CheckInAsync(int reservationId, int employeeId);
        Task<bool> CheckOutAsync(int reservationId, int employeeId);
    }

    public class BookingRepository : IBookingRepository
    {
        private readonly IDbConnection _db; 
        public BookingRepository(IDbConnection db) => _db = db;

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            const string sql = "SELECT * FROM Reservation";
            return await _db.QueryAsync<Reservation>(sql);
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Reservation WHERE Id = @id";
            return await _db.QueryFirstOrDefaultAsync<Reservation>(sql, new { id }); //null if no rows are found.
        }

        public async Task<int> CreateReservationAsync(Reservation reservation)
        {
            const string sql = @"
            INSERT INTO Reservation (GuestId, RoomId, CheckInDate, CheckOutDate, Status)
            VALUES (@GuestId, @RoomId, @CheckInDate, @CheckOutDate, @Status);
            SELECT CAST(SCOPE_IDENTITY() as int)";  
            return await RepositoryHelper.ExecuteWithHandlingAsync(
               () => _db.ExecuteScalarAsync<int>(sql, reservation),
               "Failed to create room type."); 
        }

        public async Task<bool> UpdateReservationAsync(Reservation reservation)
        {
            const string sql = @"
            UPDATE Reservation
            SET CheckInDate = @CheckInDate,
                CheckOutDate = @CheckOutDate,
                Status = @Status
            WHERE Id = @Id";  
            var rows = await RepositoryHelper.ExecuteWithHandlingAsync(
                 () => _db.ExecuteAsync(sql, reservation),
                 "Failed to create room type.");
                 return rows > 0;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            const string sql = "DELETE FROM Reservation WHERE Id = @id"; 
            var rows = await RepositoryHelper.ExecuteWithHandlingAsync(
                 () => _db.ExecuteAsync(sql, new { id }),
                 "Failed to create room type.");
                 return rows > 0;
        }

        public async Task<IEnumerable<RoomRate>> GetAllRoomRatesAsync()
        {
            var sql = "SELECT * FROM RoomRate ORDER BY ValidFrom DESC";
            return await _db.QueryAsync<RoomRate>(sql);
        }

        public async Task<RoomRate?> GetRoomRateByIdAsync(int id)
        {
            var sql = "SELECT * FROM RoomRate WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<RoomRate>(sql, new { Id = id });
        }

        public async Task<int> CreateRoomRateAsync(RoomRate rate)
        {
            var sql = @"
            INSERT INTO RoomRate (RoomTypeId, PricePerNight, ValidFrom, ValidTo)
            VALUES (@RoomTypeId, @PricePerNight, @ValidFrom, @ValidTo);
            SELECT CAST(SCOPE_IDENTITY() as int);"; 
            return await RepositoryHelper.ExecuteWithHandlingAsync(
                 () => _db.ExecuteScalarAsync<int>(sql, rate),
                 "Failed to create room type."); 
        }

        public async Task<bool> UpdateRoomRateAsync(RoomRate rate)
        {
            var sql = @"
            UPDATE RoomRate SET
                RoomTypeId = @RoomTypeId,
                PricePerNight = @PricePerNight,
                ValidFrom = @ValidFrom,
                ValidTo = @ValidTo
            WHERE Id = @Id";  
            var affected = await RepositoryHelper.ExecuteWithHandlingAsync(
                 () => _db.ExecuteAsync(sql, rate),
                 "Failed to create room type.");
                    return affected > 0;
        }

        public async Task<bool> DeleteRoomRateAsync(int id)
        {
            var sql = "DELETE FROM RoomRate WHERE Id = @Id";  
            var affected = await RepositoryHelper.ExecuteWithHandlingAsync(
                   () => _db.ExecuteAsync(sql, new { Id = id }),
                   "Failed to create room type.");
                    return affected > 0;
        }

        public async Task<bool> CheckInAsync(int reservationId, int employeeId)
        {
            var sql = @"
            INSERT INTO CheckIn (ReservationId, PerformedByEmployeeId, Timestamp)
            VALUES (@ReservationId, @EmployeeId, GETDATE());

            UPDATE Reservation SET Status = 'CheckedIn' WHERE Id = @ReservationId;"; 

            var affected = await RepositoryHelper.ExecuteWithHandlingAsync(
                   () => _db.ExecuteAsync(sql, new { ReservationId = reservationId, EmployeeId = employeeId }),
                   "Failed to create room type.");
                        return affected > 0;
        }

        public async Task<bool> CheckOutAsync(int reservationId, int employeeId)
        {
            var sql = @"
            INSERT INTO CheckOut (ReservationId, PerformedByEmployeeId, Timestamp)
            VALUES (@ReservationId, @EmployeeId, GETDATE());

            UPDATE Reservation SET Status = 'CheckedOut' WHERE Id = @ReservationId;"; 

            var affected = await RepositoryHelper.ExecuteWithHandlingAsync(
                   () => _db.ExecuteAsync(sql, new { ReservationId = reservationId, EmployeeId = employeeId }),
                   "Failed to create room type.");
                        return affected > 0;
        }
    }
}
