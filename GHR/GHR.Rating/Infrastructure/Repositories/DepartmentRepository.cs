namespace GHR.Rating.Infrastructure.Repositories
{
    using System.Data;

    using Dapper;
    using GHR.Rating.Domain.Repositories;  
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnection _db;
        public DepartmentRepository(IDbConnection db) => _db = db;

        public async Task<bool> Exists(int departmentId)
        {
            var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Departments WHERE Id = @Id) THEN 1 ELSE 0 END";
            var exists = await _db.ExecuteScalarAsync<bool>(sql, new { Id = departmentId });
            return exists;
        }

    }
}
