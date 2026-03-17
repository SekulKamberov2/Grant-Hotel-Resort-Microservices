namespace GHR.Rating.Infrastructure.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;

    using Dapper;

    using GHR.Rating.Application.Commands.CreateAward;
    using GHR.Rating.Application.Commands.UpdateAward;
    using GHR.Rating.Domain.Entities;
    using GHR.Rating.Domain.Repositories;

    public class AwardRepository : IAwardRepository
    {
        private readonly string _connectionString;

        public AwardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<int> InsertAwardAsync(CreateAwardCommand command)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO Awards (UsersId, DepartmentId, Title, Period, Date)
                VALUES (@UsersId, @DepartmentId, @Title, @Period, @Date);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, command);
        }

        public async Task<bool> AwardExistsAsync(int id)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Awards WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task DeleteAwardAsync(int id)
        {
            using var connection = CreateConnection();
            const string sql = "DELETE FROM Awards WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task UpdateAwardAsync(UpdateAwardCommand command)
        {
            using var connection = CreateConnection();
            const string sql = @"
                UPDATE Awards
                SET UsersId = @UsersId,
                    DepartmentId = @DepartmentId,
                    Title = @Title,
                    Period = @Period,
                    Date = @Date
                WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, command);
        }

        public async Task<Award?> GetAwardByIdAsync(int id)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT * FROM Awards WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Award>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Award>> GetAwardsByPeriodAsync(string period)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Awards WHERE Period = @Period";
            return await connection.QueryAsync<Award>(sql, new { Period = period });
        }

        public async Task<IEnumerable<(int UserId, int DepartmentId)>> GetTopPerformersByPeriodAsync(string period)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT TOP 10 UsersId, DepartmentId
                FROM Awards
                WHERE Period = @Period
                GROUP BY UsersId, DepartmentId";
            return await connection.QueryAsync<(int UserId, int DepartmentId)>(sql, new { Period = period });
        }
    }
}
