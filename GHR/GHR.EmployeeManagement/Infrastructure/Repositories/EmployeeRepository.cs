namespace GHR.EmployeeManagement.Infrastructure.Repositories
{
    using System.Data;
    using Microsoft.Data.SqlClient;

    using Dapper;
    using GHR.EmployeeManagement.Domain.Entities;

    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> SearchByNameAsync(string name);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetByFacilityAsync(int facilityId);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
        Task<IEnumerable<Employee>> GetByManagerAsync(int managerId);
        Task<IEnumerable<Employee>> GetBirthdaysThisMonthAsync();
        Task<IEnumerable<Employee>> GetByHireDateBeforeAsync(DateTime date); 
        Task<IEnumerable<Employee>> GetByStatusAsync(string status);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _dbConnection;  
        public EmployeeRepository(IConfiguration configuration) =>
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public async Task<IEnumerable<Employee>> GetAllAsync()
        { 
            var sql = "SELECT * FROM Employees";
            return await _dbConnection.QueryAsync<Employee>(sql);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        { 
            var sql = "SELECT * FROM Employees WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Employee>> SearchByNameAsync(string name)
        { 
            var sql = "SELECT * FROM Employees WHERE Name LIKE @Name";
            return await _dbConnection.QueryAsync<Employee>(sql, new { Name = $"%{name}%" });
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
        { 
            var sql = "SELECT * FROM Employees WHERE DepartmentId = @DepartmentId";
            return await _dbConnection.QueryAsync<Employee>(sql, new { DepartmentId = departmentId });
        }

        public async Task<IEnumerable<Employee>> GetByFacilityAsync(int facilityId)
        {
            
            var sql = "SELECT * FROM Employees WHERE FacilityId = @FacilityId";
            return await _dbConnection.QueryAsync<Employee>(sql, new { FacilityId = facilityId });
        }

        public async Task AddAsync(Employee employee)
        {
            try
            { 
                var sql = @"
                    INSERT INTO Employees (Id, Name, Email, DepartmentId, FacilityId)
                    VALUES (@Id, @Name, @Email, @DepartmentId, @FacilityId)";
            await _dbConnection.ExecuteAsync(sql, employee);
            }
            catch
            { 
                throw;   
            }
        }

        public async Task UpdateAsync(Employee employee)
        {
            try
            {
                var sql = @"
                    UPDATE Employees
                    SET Name = @Name,
                        Email = @Email,
                        DepartmentId = @DepartmentId,
                        FacilityId = @FacilityId
                    WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sql, employee);
            }
            catch
            {
                throw; 
            }
        }

        public async Task DeleteAsync(Employee employee)
        {
            try
            {
                var sql = "DELETE FROM Employees WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = employee.Id });
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetByManagerAsync(int managerId)
        {
            var sql = @"SELECT * FROM Employees WHERE ManagerId = @ManagerId";
            var employees = await _dbConnection.QueryAsync<Employee>(sql, new { ManagerId = managerId });
            return employees;
        }

        public async Task<IEnumerable<Employee>> GetByStatusAsync(string status)
        {
            var sql = @"SELECT * FROM Employees WHERE Status = @Status";
            var employees = await _dbConnection.QueryAsync<Employee>(sql, new { Status = status });
            return employees;
        }

        public async Task<IEnumerable<Employee>> GetBirthdaysThisMonthAsync()
        {
            var currentMonth = DateTime.UtcNow.Month;

            var sql = @"SELECT * FROM Employees WHERE MONTH(DateOfBirth) = @Month";

            var employees = await _dbConnection.QueryAsync<Employee>(sql, new { Month = currentMonth });
            return employees;
        }

        public async Task<IEnumerable<Employee>> GetByHireDateBeforeAsync(DateTime date)
        {
            var sql = "SELECT * FROM Employees WHERE HireDate < @Date";
            var employees = await _dbConnection.QueryAsync<Employee>(sql, new { Date = date });
            return employees;
        }
    } 
}
