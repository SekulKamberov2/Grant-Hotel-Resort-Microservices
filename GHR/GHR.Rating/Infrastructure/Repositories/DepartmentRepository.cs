namespace GHR.Rating.Infrastructure.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;
    using Dapper;
    using GHR.Rating.Domain.Repositories;
 

    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly string _connectionString;

        public DepartmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<bool> Exists(int departmentId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Departments WHERE Id = @Id) THEN 1 ELSE 0 END";
            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Id = departmentId });
            return exists;
        }
    }
}
