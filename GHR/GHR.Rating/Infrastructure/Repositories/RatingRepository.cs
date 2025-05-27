namespace GHR.Rating.Infrastructure.Repositories
{
    using Dapper;
    using GHR.Rating.Domain.Entities; 
    using GHR.Rating.Domain.Repositories;
    using GHR.SharedKernel.Exceptions;
    using System.Data;
    using System.Text;
    using System.Text.Json;

    public class RatingRepository : IRatingRepository
    {
        private readonly IDbConnection _db; 
        public RatingRepository(IDbConnection db) => _db = db; 
        public async Task<bool> UserHasRecentRating(int userId, int serviceId)
        {
            const string sql = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM Ratings 
                    WHERE UserId = @UserId 
                      AND ServiceId = @ServiceId 
                      AND RatingDate >= DATEADD(day, -1, GETDATE())
                      AND IsDeleted = 0
                ) THEN 1 ELSE 0 END";

            return await _db.ExecuteScalarAsync<bool>(sql, new { UserId = userId, ServiceId = serviceId });
        }

        public async Task<int> AddAsync(Rating rating)
        {
            const string sql = @"
                INSERT INTO 
                    Ratings (UserId, ServiceId, DepartmentId, Stars, Comment, RatingDate)
                    VALUES (@UserId, @ServiceId, @DepartmentId, @Stars, @Comment, @RatingDate)";
            try
            { 
                var result = await _db.ExecuteAsync(sql, rating);
                var options = new JsonSerializerOptions { WriteIndented = true }; 
                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error inserting rating", ex);
            }
        }

        public async Task<double> GetAverageRatingByDepartmentAsync(int departmentId)
        {
            const string sql = @"SELECT AVG(CAST(Stars AS FLOAT)) FROM Ratings WHERE DepartmentId = @DepartmentId";
            return await _db.ExecuteScalarAsync<double>(sql, new { DepartmentId = departmentId });
        }

        public async Task<Rating?> GetByIdAsync(int id)
        {
            const string sql = @"SELECT * FROM Ratings WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<Rating>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Ratings ORDER BY RatingDate DESC";
            return await _db.QueryAsync<Rating>(sql);
        }

        public async Task<IEnumerable<Rating>> GetByUserAsync(int userId)
        {
            const string sql = @"SELECT * FROM Ratings WHERE UserId = @UserId ORDER BY RatingDate DESC";
            return await _db.QueryAsync<Rating>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Rating>> GetByDepartmentAsync(int departmentId)
        {
            const string sql = @"SELECT * FROM Ratings WHERE DepartmentId = @DepartmentId ORDER BY RatingDate DESC";
            return await _db.QueryAsync<Rating>(sql, new { DepartmentId = departmentId });
        }

        public async Task<IEnumerable<Rating>> GetByServiceAsync(int serviceId)
        {
            const string sql = @"SELECT * FROM Ratings WHERE ServiceId = @ServiceId ORDER BY RatingDate DESC";
            return await _db.QueryAsync<Rating>(sql, new { ServiceId = serviceId });
        }

        public async Task<IEnumerable<Rating>> GetRatingsFromDateAsync(DateTime startDate)
        {
            const string sql = @"SELECT * FROM Ratings WHERE RatingDate >= @StartDate";
            return await _db.QueryAsync<Rating>(sql, new { StartDate = startDate });
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                return await _db.ExecuteAsync("DELETE FROM Ratings WHERE Id = @Id", new { Id = id });
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting rating", ex);
            }
        }

        public async Task<bool> ExistsAsync(int ratingId)
        {
            const string sql = "SELECT COUNT(1) FROM Ratings WHERE Id = @Id";
            var count = await _db.ExecuteScalarAsync<int>(sql, new { Id = ratingId });
            return count > 0;
        }

        public async Task<bool> MarkAsApprovedAsync(int ratingId)
        {
            const string sql = @"UPDATE Ratings SET IsApproved = 1 WHERE Id = @Id";
            var affected = await _db.ExecuteAsync(sql, new { Id = ratingId });
            return affected > 0;
        }

        public async Task<int> BulkDeleteAsync(IEnumerable<int> ratingIds)
        {
            const string sql = @"UPDATE Ratings SET IsDeleted = 1 WHERE Id IN @Ids";
            return await _db.ExecuteAsync(sql, new { Ids = ratingIds });
        }

        public async Task<bool> FlagAsync(int ratingId, string reason)
        {
            const string sql = @"UPDATE Ratings 
                                 SET IsFlagged = 1, FlagReason = @Reason 
                                 WHERE Id = @Id";
            return await _db.ExecuteAsync(sql, new { Id = ratingId, Reason = reason }) > 0;
        }

        public async Task<bool> UpdateAsync(int id, int stars, string comment)
        {
            const string sql = @"UPDATE Ratings 
                                 SET Stars = @Stars, Comment = @Comment, RatingDate = GETDATE() 
                                 WHERE Id = @Id";
            return await _db.ExecuteAsync(sql, new { Id = id, Stars = stars, Comment = comment }) > 0;
        }

        public async Task<IEnumerable<Rating>> GetByStatusAsync(bool? isApproved, bool? isFlagged, bool? isDeleted)
        {
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

            return await _db.QueryAsync<Rating>(sql.ToString(), parameters);
        }

        public async Task<bool> RestoreAsync(int ratingId)
        {
            const string sql = "UPDATE Ratings SET IsDeleted = 0 WHERE Id = @Id AND IsDeleted = 1";
            return await _db.ExecuteAsync(sql, new { Id = ratingId }) > 0;
        }

        public async Task<bool> UnflagAsync(int ratingId)
        {
            const string sql = "UPDATE Ratings SET IsFlagged = 0, FlagReason = NULL WHERE Id = @Id AND IsFlagged = 1";
            return await _db.ExecuteAsync(sql, new { Id = ratingId }) > 0;
        }
    }
}
