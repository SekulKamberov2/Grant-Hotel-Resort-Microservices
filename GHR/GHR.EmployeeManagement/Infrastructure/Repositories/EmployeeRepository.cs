namespace GHR.EmployeeManagement.Infrastructure.Repositories
{
    using Dapper;
    using GHR.EmployeeManagement.Domain.Entities;
    using Microsoft.Data.SqlClient;
    using System.Data;
    using System.Text.Json;

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

        public async Task<Employee?> GetByUserIdAsync(int userId)
        { 
            var sql = "SELECT * FROM Employees WHERE UserId = @UserId";
            return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Employee>> SearchByNameAsync(string name)
        {
            var sql = @"
                SELECT * FROM Employees
                WHERE FirstName LIKE @Name OR LastName LIKE @Name
                ORDER BY
                    CASE 
                        WHEN FirstName = @ExactName THEN 0
                        WHEN LastName = @ExactName THEN 1
                        ELSE 2
                    END,
                    FirstName, LastName
                ";

            return await _dbConnection.QueryAsync<Employee>(sql, new { Name = $"%{name}%", ExactName = name });
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

        public async Task<Employee> AddAsync(Employee employee)
        {
            try
            {
                var sql = @"
            INSERT INTO Employees (FirstName, LastName, DateOfBirth, Gender, HireDate,
                DepartmentId, FacilityId, JobTitle, Email, PhoneNumber, Address, Salary, Status, 
                ManagerId, EmergencyContact, Notes)
            VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @HireDate,
                @DepartmentId, @FacilityId, @JobTitle, @Email, @PhoneNumber, @Address, @Salary, @Status, 
                @ManagerId, @EmergencyContact, @Notes);
            SELECT CAST(SCOPE_IDENTITY() as int);";

                var id = await _dbConnection.ExecuteScalarAsync<int>(sql, employee);
                employee.Id = id;
                return employee;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting employee: " + ex.Message);
                throw;  
            }
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            try
            {
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

                var rowsAffected = await _dbConnection.ExecuteAsync(sql, employee);

                if (rowsAffected == 0)
                    throw new Exception($"No employee found with Id = {employee.Id}");

                return employee;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating employee: " + ex.Message);
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
