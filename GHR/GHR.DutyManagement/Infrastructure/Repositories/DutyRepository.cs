namespace GHR.DutyManagement.Infrastructure.Repositories
{
    using System.Data; 
    using Microsoft.Data.SqlClient;

    using Dapper; 

    using GHR.DutyManagement.Application.Interfaces;
    using SharedKernel;

    public class DutyRepository : IDutyRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<DutyRepository> _logger; 
        public DutyRepository(IConfiguration config, ILogger<DutyRepository> logger)
        {
            _connection = new SqlConnection(config.GetConnectionString("SqlServer"));
            _logger = logger;
        }

        public async Task<IdentityResult<bool>> GetTaskByNameAsync(string task)
        {  
            return await ExecuteLogging.ExecuteWithLogging(
                async () =>
                {
                    var sql = "SELECT COUNT(1) FROM Duties WHERE Title = @TitleTask";
                    var count = await _connection.ExecuteScalarAsync<int>(sql, new { TitleTask = task });
                    return count > 0;
                },
                _logger,
                "Task created successfully.",
                "An error occurred while creating the Task."
            );
        }

        public async Task<IdentityResult<bool>> CreateDutyAsync(int employeeId, string task)
        {
            return await ExecuteLogging.ExecuteWithLogging(
                async () =>
                {
                    var sql = "INSERT INTO Duties (Id, Task) VALUES (@EmployeeId, @Task)";
                    var rows = await _connection.ExecuteAsync(sql, new { Id = employeeId, Task = task });
                    return rows > 0;
                },
                _logger,
                "User {Email} created successfully.",
                "An error occurred while creating the user with email {Email}."
            );
        }
    }
}
