namespace GHR.RoomManagement.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;

    using Dapper; 
    using GHR.RoomManagement.Entities;

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
        private readonly string _connectionString;

        public BookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            using var connection = CreateConnection();
            const string sql = "SELECT * FROM Reservation";
            return await connection.QueryAsync<Reservation>(sql);
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT * FROM Reservation WHERE Id = @id";
            return await connection.QueryFirstOrDefaultAsync<Reservation>(sql, new { id });
        }

        public async Task<int> CreateReservationAsync(Reservation reservation)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO Reservation (GuestId, RoomId, CheckInDate, CheckOutDate, Status)
                VALUES (@GuestId, @RoomId, @CheckInDate, @CheckOutDate, @Status);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, reservation);
        }

        public async Task<bool> UpdateReservationAsync(Reservation reservation)
        {
            using var connection = CreateConnection();
            const string sql = @"
                UPDATE Reservation
                SET CheckInDate = @CheckInDate,
                    CheckOutDate = @CheckOutDate,
                    Status = @Status
                WHERE Id = @Id";
            var rows = await connection.ExecuteAsync(sql, reservation);
            return rows > 0;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            using var connection = CreateConnection();
            const string sql = "DELETE FROM Reservation WHERE Id = @id";
            var rows = await connection.ExecuteAsync(sql, new { id });
            return rows > 0;
        }

        public async Task<IEnumerable<RoomRate>> GetAllRoomRatesAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM RoomRate ORDER BY ValidFrom DESC";
            return await connection.QueryAsync<RoomRate>(sql);
        }

        public async Task<RoomRate?> GetRoomRateByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM RoomRate WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<RoomRate>(sql, new { Id = id });
        }

        public async Task<int> CreateRoomRateAsync(RoomRate rate)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO RoomRate (RoomTypeId, PricePerNight, ValidFrom, ValidTo)
                VALUES (@RoomTypeId, @PricePerNight, @ValidFrom, @ValidTo);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, rate);
        }

        public async Task<bool> UpdateRoomRateAsync(RoomRate rate)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE RoomRate SET
                    RoomTypeId = @RoomTypeId,
                    PricePerNight = @PricePerNight,
                    ValidFrom = @ValidFrom,
                    ValidTo = @ValidTo
                WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(sql, rate);
            return affected > 0;
        }

        public async Task<bool> DeleteRoomRateAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM RoomRate WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        public async Task<bool> CheckInAsync(int reservationId, int employeeId)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO CheckIn (ReservationId, PerformedByEmployeeId, Timestamp)
                VALUES (@ReservationId, @EmployeeId, GETDATE());
                UPDATE Reservation SET Status = 'CheckedIn' WHERE Id = @ReservationId;";
            var affected = await connection.ExecuteAsync(sql, new { ReservationId = reservationId, EmployeeId = employeeId });
            return affected > 0;
        }

        public async Task<bool> CheckOutAsync(int reservationId, int employeeId)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO CheckOut (ReservationId, PerformedByEmployeeId, Timestamp)
                VALUES (@ReservationId, @EmployeeId, GETDATE());
                UPDATE Reservation SET Status = 'CheckedOut' WHERE Id = @ReservationId;";
            var affected = await connection.ExecuteAsync(sql, new { ReservationId = reservationId, EmployeeId = employeeId });
            return affected > 0;
        }
    }
}
