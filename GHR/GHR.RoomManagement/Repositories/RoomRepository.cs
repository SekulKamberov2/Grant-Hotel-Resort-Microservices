namespace GHR.RoomManagement.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;
 
    using Dapper;

    using GHR.RoomManagement.DTOs;
    using GHR.RoomManagement.Entities; 
  
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync(string? status, int? floor, string? type);
        Task<Room?> GetByIdAsync(int id);
        Task<int> CreateAsync(Room room);
        Task<bool> UpdateAsync(int id, Room room);
        Task<bool> DeleteAsync(int id); 
        Task<IEnumerable<RoomType>> GetAllRoomTypesAsync();
        Task<int> CreateRoomTypeAsync(RoomType type);
        Task<bool> UpdateRoomTypeAsync(RoomType type);
        Task<bool> DeleteRoomTypeAsync(int id); 
        Task<IEnumerable<RoomAvailabilityDTO>> GetAllAvailableRoomsAsync(DateTime? startDate, DateTime? endDate, string? type);
        Task<RoomAvailabilityDTO?> GetRoomAvailabilityByIdAsync(int roomId);
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Room>> GetAllAsync(string? status, int? floor, string? type)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT r.Id, r.RoomNumber, r.Floor, r.TypeId, r.Status, r.Description, rt.Name AS TypeName
                FROM Room r
                INNER JOIN RoomType rt ON r.TypeId = rt.Id
                WHERE (@Floor IS NULL OR r.Floor = @Floor)
                  AND (@Status IS NULL OR r.Status = @Status)
                  AND (@Type IS NULL OR rt.Name = @Type)";
            return await connection.QueryAsync<Room>(sql, new { Floor = floor, Status = status, Type = type });
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Room WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Room>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Room room)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO Room (RoomNumber, Floor, TypeId, Status, Description)
                VALUES (@RoomNumber, @Floor, @TypeId, @Status, @Description);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, room);
        }

        public async Task<bool> UpdateAsync(int id, Room room)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE Room SET 
                    RoomNumber = @RoomNumber,
                    Floor = @Floor,
                    TypeId = @TypeId,
                    Status = @Status,
                    Description = @Description
                WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                room.RoomNumber,
                room.Floor,
                room.TypeId,
                room.Status,
                room.Description,
                Id = id
            });
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Room WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<RoomType>> GetAllRoomTypesAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM RoomType";
            return await connection.QueryAsync<RoomType>(sql);
        }

        public async Task<int> CreateRoomTypeAsync(RoomType type)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO RoomType (Name, Description)
                VALUES (@Name, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, type);
        }

        public async Task<bool> UpdateRoomTypeAsync(RoomType type)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE RoomType
                SET Name = @Name, Description = @Description
                WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(sql, type);
            return affected > 0;
        }

        public async Task<bool> DeleteRoomTypeAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM RoomType WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        public async Task<IEnumerable<RoomAvailabilityDTO>> GetAllAvailableRoomsAsync(DateTime? startDate, DateTime? endDate, string? type)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT r.Id AS RoomId, r.RoomNumber, rt.Name AS Type, r.Floor, r.Status,
                       rr.PricePerNight
                FROM Room r
                JOIN RoomType rt ON r.TypeId = rt.Id
                JOIN RoomRate rr ON rr.RoomTypeId = rt.Id
                WHERE r.Status = 'Available'
                  AND (@type IS NULL OR rt.Name = @type)
                  AND (@startDate IS NULL OR NOT EXISTS (
                       SELECT 1 FROM Reservation
                       WHERE RoomId = r.Id
                         AND Status IN ('Confirmed', 'CheckedIn')
                         AND (
                             (CheckInDate BETWEEN @startDate AND @endDate) OR
                             (CheckOutDate BETWEEN @startDate AND @endDate)
                         )
                  ))
                  AND (@startDate IS NULL OR rr.ValidFrom <= @startDate AND (rr.ValidTo IS NULL OR rr.ValidTo >= @endDate))";
            return await connection.QueryAsync<RoomAvailabilityDTO>(sql, new { startDate, endDate, type });
        }

        public async Task<RoomAvailabilityDTO?> GetRoomAvailabilityByIdAsync(int roomId)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT TOP 1 r.Id AS RoomId, r.RoomNumber, rt.Name AS Type, r.Floor, r.Status,
                             rr.PricePerNight
                FROM Room r
                JOIN RoomType rt ON r.TypeId = rt.Id
                LEFT JOIN RoomRate rr ON rr.RoomTypeId = rt.Id AND rr.ValidFrom <= GETDATE()
                                         AND (rr.ValidTo IS NULL OR rr.ValidTo >= GETDATE())
                WHERE r.Id = @roomId";
            return await connection.QueryFirstOrDefaultAsync<RoomAvailabilityDTO>(sql, new { roomId });
        }
    }
}
