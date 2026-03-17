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
        Task<Employee?> GetByUserIdAsync(int id);
        Task<IEnumerable<Employee>> SearchByNameAsync(string name);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetByFacilityAsync(int facilityId);
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetByManagerAsync(int managerId);
        Task<IEnumerable<Employee>> GetBirthdaysThisMonthAsync();
        Task<IEnumerable<Employee>> GetByHireDateBeforeAsync(DateTime date); 
        Task<IEnumerable<Employee>> GetByStatusAsync(string status);

        Task<IEnumerable<Employee>> GetHiredAfterAsync(DateTime date);
        Task<IEnumerable<Employee>> GetWithSalaryAboveAsync(decimal salary);
        Task<int> IncreaseSalaryForHiredBeforeAsync(DateTime cutoffDate, decimal percentage);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees";
            return await connection.QueryAsync<Employee>(sql);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
        }

        public async Task<Employee?> GetByUserIdAsync(int userId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE UserId = @UserId";
            return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Employee>> SearchByNameAsync(string name)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT * FROM Employees
                WHERE FirstName LIKE @Name OR LastName LIKE @Name
                ORDER BY
                    CASE 
                        WHEN FirstName = @ExactName THEN 0
                        WHEN LastName = @ExactName THEN 1
                        ELSE 2
                    END,
                    FirstName, LastName";
            return await connection.QueryAsync<Employee>(sql, new { Name = $"%{name}%", ExactName = name });
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE DepartmentId = @DepartmentId";
            return await connection.QueryAsync<Employee>(sql, new { DepartmentId = departmentId });
        }

        public async Task<IEnumerable<Employee>> GetByFacilityAsync(int facilityId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE FacilityId = @FacilityId";
            return await connection.QueryAsync<Employee>(sql, new { FacilityId = facilityId });
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO Employees (FirstName, LastName, DateOfBirth, Gender, HireDate,
                    DepartmentId, FacilityId, JobTitle, Email, PhoneNumber, Address, Salary, Status, 
                    ManagerId, EmergencyContact, Notes)
                VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @HireDate,
                    @DepartmentId, @FacilityId, @JobTitle, @Email, @PhoneNumber, @Address, @Salary, @Status, 
                    @ManagerId, @EmergencyContact, @Notes);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = await connection.ExecuteScalarAsync<int>(sql, employee);
            employee.Id = id;
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE Employees SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    DateOfBirth = @DateOfBirth,
                    Gender = @Gender,
                    HireDate = @HireDate,
                    DepartmentId = @DepartmentId,
                    FacilityId = @FacilityId,
                    JobTitle = @JobTitle,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    Address = @Address,
                    Salary = @Salary,
                    Status = @Status,
                    ManagerId = @ManagerId,
                    EmergencyContact = @EmergencyContact,
                    Notes = @Notes
                WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, employee);
            if (rowsAffected == 0)
                throw new Exception($"No employee found with Id = {employee.Id}");
            return employee;
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Employees WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Employee>> GetByManagerAsync(int managerId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE ManagerId = @ManagerId";
            return await connection.QueryAsync<Employee>(sql, new { ManagerId = managerId });
        }

        public async Task<IEnumerable<Employee>> GetByStatusAsync(string status)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE Status = @Status";
            return await connection.QueryAsync<Employee>(sql, new { Status = status });
        }

        public async Task<IEnumerable<Employee>> GetBirthdaysThisMonthAsync()
        {
            using var connection = CreateConnection();
            var currentMonth = DateTime.UtcNow.Month;
            var sql = "SELECT * FROM Employees WHERE MONTH(DateOfBirth) = @Month";
            return await connection.QueryAsync<Employee>(sql, new { Month = currentMonth });
        }

        public async Task<IEnumerable<Employee>> GetByHireDateBeforeAsync(DateTime date)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE HireDate < @Date";
            return await connection.QueryAsync<Employee>(sql, new { Date = date });
        }

        public async Task<IEnumerable<Employee>> GetHiredAfterAsync(DateTime date)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE HireDate >= @Date";
            return await connection.QueryAsync<Employee>(sql, new { Date = date });
        }

        public async Task<IEnumerable<Employee>> GetWithSalaryAboveAsync(decimal salary)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Employees WHERE Salary > @Salary";
            return await connection.QueryAsync<Employee>(sql, new { Salary = salary });
        }

        public async Task<int> IncreaseSalaryForHiredBeforeAsync(DateTime cutoffDate, decimal percentage)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE Employees
                SET Salary = Salary * (1 + @Percentage / 100.0)
                WHERE HireDate < @CutoffDate";
            return await connection.ExecuteAsync(sql, new { CutoffDate = cutoffDate, Percentage = percentage });
        }
    }
}
