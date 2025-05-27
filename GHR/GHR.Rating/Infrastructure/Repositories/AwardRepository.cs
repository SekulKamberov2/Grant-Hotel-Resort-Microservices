namespace GHR.Rating.Infrastructure.Repositories
{
    using Dapper;
    using GHR.Rating.Application.Commands.CreateAward;
    using GHR.Rating.Application.Commands.UpdateAward;
    using GHR.Rating.Domain.Entities;
    using GHR.Rating.Domain.Repositories;
    using System;
    using System.Data;

    public class AwardRepository : IAwardRepository
    {
        private readonly IDbConnection _dbConnection; 
        public AwardRepository(IDbConnection dbConnection) => _dbConnection = dbConnection; 

        public async Task<int> InsertAwardAsync(CreateAwardCommand command)
        {
            const string sql = @"
                INSERT INTO Awards (UsersId, DepartmentId, Title, Period, Date)
                VALUES (@UsersId, @DepartmentId, @Title, @Period, @Date);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, command); 
        }

        public async Task<bool> AwardExistsAsync(int id)
        {
            const string sql = "SELECT COUNT(1) FROM Awards WHERE Id = @Id";
            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task DeleteAwardAsync(int id)
        {
            const string sql = "DELETE FROM Awards WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task UpdateAwardAsync(UpdateAwardCommand command)
        {
            const string sql = @"
                UPDATE Awards
                SET UsersId = @UsersId,
                    DepartmentId = @DepartmentId,
                    Title = @Title,
                    Period = @Period,
                    Date = @Date
                WHERE Id = @Id;";

            await _dbConnection.ExecuteAsync(sql, command);
        }

        public async Task<Award?> GetAwardByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Awards WHERE Id = @Id"; 
            return await _dbConnection.QueryFirstOrDefaultAsync<Award>(sql, new { Id = id });
        }
        public async Task<IEnumerable<Award>> GetAwardsByPeriodAsync(string period)
        {  
            var sql = "SELECT * FROM Awards WHERE Period = @Period"; 
            return await _dbConnection.QueryAsync<Award>(sql, new { Period = period });
        }
        public async Task<IEnumerable<(int UserId, int DepartmentId)>> GetTopPerformersByPeriodAsync(string period)
        {
            var sql = @"
                SELECT TOP 10 UsersId, DepartmentId
                FROM Awards
                WHERE Period = @Period
                GROUP BY UsersId, DepartmentId";
            return await _dbConnection.QueryAsync<(int, int)>(sql, new { Period = period }); 
        }

    }
}
