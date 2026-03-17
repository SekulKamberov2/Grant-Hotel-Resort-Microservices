namespace GHR.Rating.Infrastructure.Repositories
{
    using System.Data;
    using System.Text;
    using Microsoft.Data.SqlClient;

    using Dapper;

    using GHR.Rating.Domain.Entities; 
    using GHR.Rating.Domain.Repositories; 

    public class RatingRepository : IRatingRepository
    {
        private readonly string _connectionString;

        public RatingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<bool> UserHasRecentRating(int userId, int serviceId)
        {
            using var connection = CreateConnection();
            const string sql = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM Ratings 
                    WHERE UserId = @UserId 
                      AND ServiceId = @ServiceId 
                      AND RatingDate >= DATEADD(day, -1, GETDATE())
                      AND IsDeleted = 0
                ) THEN 1 ELSE 0 END";
            return await connection.ExecuteScalarAsync<bool>(sql, new { UserId = userId, ServiceId = serviceId });
        }

        public async Task<int> AddAsync(Rating rating)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO Ratings (UserId, ServiceId, DepartmentId, Stars, Comment, RatingDate)
                VALUES (@UserId, @ServiceId, @DepartmentId, @Stars, @Comment, @RatingDate);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, rating);
        }

        public async Task<double> GetAverageRatingByDepartmentAsync(int departmentId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT AVG(CAST(Stars AS FLOAT)) FROM Ratings WHERE DepartmentId = @DepartmentId";
            return await connection.ExecuteScalarAsync<double>(sql, new { DepartmentId = departmentId });
        }

        public async Task<Rating?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Ratings WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Rating>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            using var connection = CreateConnection();
            const string sql = "SELECT * FROM Ratings ORDER BY RatingDate DESC";
            return await connection.QueryAsync<Rating>(sql);
        }

        public async Task<IEnumerable<Rating>> GetByUserAsync(int userId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Ratings WHERE UserId = @UserId ORDER BY RatingDate DESC";
            return await connection.QueryAsync<Rating>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Rating>> GetByDepartmentAsync(int departmentId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Ratings WHERE DepartmentId = @DepartmentId ORDER BY RatingDate DESC";
            return await connection.QueryAsync<Rating>(sql, new { DepartmentId = departmentId });
        }

        public async Task<IEnumerable<Rating>> GetByServiceAsync(int serviceId)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Ratings WHERE ServiceId = @ServiceId ORDER BY RatingDate DESC";
            return await connection.QueryAsync<Rating>(sql, new { ServiceId = serviceId });
        }

        public async Task<IEnumerable<Rating>> GetRatingsFromDateAsync(DateTime startDate)
        {
            using var connection = CreateConnection();
            const string sql = @"SELECT * FROM Ratings WHERE RatingDate >= @StartDate";
            return await connection.QueryAsync<Rating>(sql, new { StartDate = startDate });
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Ratings WHERE Id = @Id", new { Id = id });
        }

        public async Task<bool> ExistsAsync(int ratingId)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Ratings WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = ratingId });
            return count > 0;
        }

        public async Task<bool> MarkAsApprovedAsync(int ratingId)
        {
            using var connection = CreateConnection();
            const string sql = @"UPDATE Ratings SET IsApproved = 1 WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = ratingId });
            return affected > 0;
        }

        public async Task<int> BulkDeleteAsync(IEnumerable<int> ratingIds)
        {
            using var connection = CreateConnection();
            const string sql = @"UPDATE Ratings SET IsDeleted = 1 WHERE Id IN @Ids";
            return await connection.ExecuteAsync(sql, new { Ids = ratingIds });
        }

        public async Task<bool> FlagAsync(int ratingId, string reason)
        {
            using var connection = CreateConnection();
            const string sql = @"UPDATE Ratings 
                                 SET IsFlagged = 1, FlagReason = @Reason 
                                 WHERE Id = @Id";
            return await connection.ExecuteAsync(sql, new { Id = ratingId, Reason = reason }) > 0;
        }

        public async Task<bool> UpdateAsync(int id, int stars, string comment)
        {
            using var connection = CreateConnection();
            const string sql = @"UPDATE Ratings 
                                 SET Stars = @Stars, Comment = @Comment, RatingDate = GETDATE() 
                                 WHERE Id = @Id";
            return await connection.ExecuteAsync(sql, new { Id = id, Stars = stars, Comment = comment }) > 0;
        }

        public async Task<IEnumerable<Rating>> GetByStatusAsync(bool? isApproved, bool? isFlagged, bool? isDeleted)
        {
            using var connection = CreateConnection();
            var sql = new StringBuilder("SELECT * FROM Ratings WHERE 1=1");
            var parameters = new DynamicParameters();

            if (isApproved.HasValue)
            {
                sql.Append(" AND IsApproved = @IsApproved");
                parameters.Add("IsApproved", isApproved.Value);
            }

            if (isFlagged.HasValue)
            {
                sql.Append(" AND IsFlagged = @IsFlagged");
                parameters.Add("IsFlagged", isFlagged.Value);
            }

            if (isDeleted.HasValue)
            {
                sql.Append(" AND IsDeleted = @IsDeleted");
                parameters.Add("IsDeleted", isDeleted.Value);
            }

            return await connection.QueryAsync<Rating>(sql.ToString(), parameters);
        }

        public async Task<bool> RestoreAsync(int ratingId)
        {
            using var connection = CreateConnection();
            const string sql = "UPDATE Ratings SET IsDeleted = 0 WHERE Id = @Id AND IsDeleted = 1";
            return await connection.ExecuteAsync(sql, new { Id = ratingId }) > 0;
        }

        public async Task<bool> UnflagAsync(int ratingId)
        {
            using var connection = CreateConnection();
            const string sql = "UPDATE Ratings SET IsFlagged = 0, FlagReason = NULL WHERE Id = @Id AND IsFlagged = 1";
            return await connection.ExecuteAsync(sql, new { Id = ratingId }) > 0;
        }
    }
}
