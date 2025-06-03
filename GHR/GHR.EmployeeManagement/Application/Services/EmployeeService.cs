namespace GHR.EmployeeManagement.Application.Services
{
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Domain.Entities;
    using GHR.EmployeeManagement.Infrastructure.Repositories;
    using GHR.SharedKernel;
    using Leaverequests.Grpc;
    using System.Text.Json;

    public interface IEmployeeService
    {
        Task<Result<IEnumerable<EmployeeDTO>>> GetAllAsync();
        Task<Result<EmployeeDTO>> GetByIdAsync(int id);
        Task<Result<EmployeeDTO>> GetByUserIdAsync(int userId);
        Task<Result<IEnumerable<EmployeeDTO>>> SearchByNameAsync(string name);
        Task<Result<IEnumerable<EmployeeDTO>>> GetByDepartmentAsync(int departmentId);
        Task<Result<IEnumerable<EmployeeDTO>>> GetByFacilityAsync(int facilityId);
        Task<Result<Employee>> CreateAsync(CreateEmployeeDTO dto);
        Task<Result<Employee>> UpdateAsync(int id, UpdateEmployeeDTO dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<IEnumerable<EmployeeDTO>>> GetHiredAfterAsync(DateTime date);
        Task<Result<IEnumerable<EmployeeDTO>>> GetSalaryAboveAsync(decimal salary);
        Task<Result<IEnumerable<EmployeeDTO>>> GetByManagerAsync(int managerId);
        Task<Result<IEnumerable<EmployeeDTO>>> GetByStatusAsync(string status);
        Task<Result<IEnumerable<EmployeeDTO>>> GetBirthdaysThisMonthAsync();
        Task<Result<bool>> IncreaseSalaryHiredBeforeAsync(int years, decimal percentage);
        Task<Result<EmployeeWithAllLeaveRequestsDTO>> GetAllLeaveRequestsByUserIdAsync(int userId);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly LeaverequestsService.LeaverequestsServiceClient _leaveRequestsClient;
        public EmployeeService(
            IEmployeeRepository repository,
            LeaverequestsService.LeaverequestsServiceClient leaveRequestsClient)
        {
            _repository = repository;
            _leaveRequestsClient = leaveRequestsClient;
        }
         
        public async Task<Result<IEnumerable<EmployeeDTO>>> GetAllAsync()
        {
            try
            {
                var employees = await _repository.GetAllAsync(); 
                if (employees == null || !employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees found", 404);

                var dtos = employees.Select(e => new EmployeeDTO
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes 
                }); 
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error.", 500);
            }
        }

        public async Task<Result<EmployeeDTO>> GetByIdAsync(int id)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(id); 
                if (employee == null)
                    return Result<EmployeeDTO>.Failure("Employee not found", 404);

                var dto = new EmployeeDTO
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    DateOfBirth = employee.DateOfBirth,
                    Gender = employee.Gender,
                    HireDate = employee.HireDate,
                    DepartmentId = employee.DepartmentId,
                    FacilityId = employee.FacilityId,
                    JobTitle = employee.JobTitle,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Address = employee.Address,
                    Salary = employee.Salary,
                    Status = employee.Status,
                    ManagerId = employee.ManagerId,
                    EmergencyContact = employee.EmergencyContact,
                    Notes = employee.Notes 
                };

                return Result<EmployeeDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<EmployeeDTO>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<EmployeeDTO>> GetByUserIdAsync(int userId)
        {
            try
            {
                var employee = await _repository.GetByUserIdAsync(userId);
                if (employee == null)
                    return Result<EmployeeDTO>.Failure("Employee not found", 404);

                var dto = new EmployeeDTO
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    DateOfBirth = employee.DateOfBirth,
                    Gender = employee.Gender,
                    HireDate = employee.HireDate,
                    DepartmentId = employee.DepartmentId,
                    FacilityId = employee.FacilityId,
                    JobTitle = employee.JobTitle,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Address = employee.Address,
                    Salary = employee.Salary,
                    Status = employee.Status,
                    ManagerId = employee.ManagerId,
                    EmergencyContact = employee.EmergencyContact,
                    Notes = employee.Notes
                };

                return Result<EmployeeDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<EmployeeDTO>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<EmployeeWithAllLeaveRequestsDTO>> GetAllLeaveRequestsByUserIdAsync(int userId)
        {
            try
            { 
                var employee = await _repository.GetByUserIdAsync(userId);  
                if (employee == null)
                    return Result<EmployeeWithAllLeaveRequestsDTO>.Failure("Employee not found", 404);
                //Console.WriteLine("CreateAsync Service BEFORE=========================================>");
                //string jsonString = JsonSerializer.Serialize(employee, new JsonSerializerOptions { WriteIndented = true });
                //Console.WriteLine(jsonString);
                var dto = new EmployeeWithAllLeaveRequestsDTO
                { 
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    DateOfBirth = employee.DateOfBirth,
                    Gender = employee.Gender,
                    HireDate = employee.HireDate,
                    DepartmentId = employee.DepartmentId,
                    FacilityId = employee.FacilityId,
                    JobTitle = employee.JobTitle,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Address = employee.Address,
                    Salary = employee.Salary,
                    Status = employee.Status,
                    ManagerId = employee.ManagerId,
                    EmergencyContact = employee.EmergencyContact,
                    Notes = employee.Notes
                };
                var request = new GetLeaveRequest();
                request.UserId = userId;
                var allLeaveRequests = await _leaveRequestsClient.GetLeaveRequestsByEmployeeAsync(request); // gRPC call
             
                if (allLeaveRequests != null)
                {
                    dto.AllLeaveRequests = allLeaveRequests.Leaves.Select(l => new LeaveRequestDTO
                    {  
                        Department = l.Department,   // Bar = 1, Hotel = 2, Restaurant = 3, Casino = 4, Beach = 5, Fitness = 6, Disco = 7 
                        LeaveTypeId = l.LeaveTypeId,
                        StartDate = string.IsNullOrEmpty(l.StartDate) ? (DateTime?)null : DateTime.Parse(l.StartDate),
                        EndDate = string.IsNullOrEmpty(l.EndDate) ? (DateTime?)null : DateTime.Parse(l.EndDate),
                        TotalDays = decimal.TryParse(l.TotalDays, out var totalDaysValue) ? totalDaysValue : (decimal?)null,
                        Reason = l.Reason,
                        ApproverId = l.ApproverId,
                        DecisionDate = string.IsNullOrEmpty(l.DecisionDate) ? (DateTime?)null : DateTime.Parse(l.DecisionDate),
                        RequestedAt = string.IsNullOrEmpty(l.RequestedAt) ? (DateTime?)null : DateTime.Parse(l.RequestedAt)
                    });
                }

                return Result<EmployeeWithAllLeaveRequestsDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<EmployeeWithAllLeaveRequestsDTO>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> SearchByNameAsync(string name)
        {
            try
            { 
                var employees = await _repository.SearchByNameAsync(name);
                 
                if (employees == null || !employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees found with the given name", 404);
                 
                var dtos = employees.Select(e => new EmployeeDTO
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes 
                });
                 
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            { 
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetByDepartmentAsync(int departmentId)
        {
            try
            { 
                var employees = await _repository.GetByDepartmentAsync(departmentId);
                 
                if (employees == null || !employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees found in this department", 404);
                 
                var dtos = employees.Select(e => new EmployeeDTO
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes 
                });
                 
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            { 
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error.", 500);
            }
        } 

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetByFacilityAsync(int facilityId)
        {
            try
            {
                var employees = await _repository.GetByFacilityAsync(facilityId);

                if (employees == null || !employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees found for the given facility", 404);

                var dtos = employees.Select(e => new EmployeeDTO
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes 
                });

                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error", 500);
            }
        } 
        public async Task<Result<Employee>> CreateAsync(CreateEmployeeDTO dto)
        {
            try
            {
                var employee = new Employee
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    HireDate = dto.HireDate,
                    DepartmentId = dto.DepartmentId,
                    FacilityId = dto.FacilityId,
                    JobTitle = dto.JobTitle,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    Salary = dto.Salary,
                    Status = dto.Status,
                    ManagerId = dto.ManagerId,
                    EmergencyContact = dto.EmergencyContact,
                    Notes = dto.Notes  
                };
              
                var createdEmployee = await _repository.AddAsync(employee); 
                if(createdEmployee != null)
                    return Result<Employee>.Success(createdEmployee);

                return Result<Employee>.Failure("Failed to create employee");
            }
            catch (Exception ex)
            {
                return Result<Employee>.Failure($"Failed to create employee", 500);
            }
        }
         
        public async Task<Result<Employee>> UpdateAsync(int id, UpdateEmployeeDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return Result<Employee>.Failure("Employee not found", 404);

                var employee = await _repository.GetByIdAsync(id);
                if (employee == null)
                    return Result<Employee>.Failure("Employee not found", 404);

                employee.FirstName = dto.FirstName;
                employee.LastName = dto.LastName;
                employee.DateOfBirth = dto.DateOfBirth;
                employee.Gender = dto.Gender;
                employee.HireDate = dto.HireDate;
                employee.DepartmentId = dto.DepartmentId;
                employee.FacilityId = dto.FacilityId;
                employee.JobTitle = dto.JobTitle;
                employee.Email = dto.Email;
                employee.PhoneNumber = dto.PhoneNumber;
                employee.Address = dto.Address;
                employee.Salary = dto.Salary;
                employee.Status = dto.Status;
                employee.ManagerId = dto.ManagerId;
                employee.EmergencyContact = dto.EmergencyContact;
                employee.Notes = dto.Notes; 

               var updatedEmployee = await _repository.UpdateAsync(employee);
                if (updatedEmployee == null)
                    return Result<Employee>.Failure($"Failed to update employee.", 500);

                return Result<Employee>.Success(updatedEmployee);
            }
            catch (Exception ex)
            {
                return Result<Employee>.Failure($"Failed to update employee.", 500);
            }
        }  
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(id);
                if (employee == null)
                    return Result<bool>.Failure("Employee not found", 404);

                await _repository.DeleteAsync(employee);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to delete employee.", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetHiredAfterAsync(DateTime date)
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                var filtered = employees.Where(e => e.HireDate >= date).ToList();

                if (!filtered.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees hired after the specified date", 404);

                var dtos = filtered.Select(e => new EmployeeDTO
                {
                     FirstName = e.FirstName,
                     LastName = e.LastName,
                     DateOfBirth = e.DateOfBirth,
                     Gender = e.Gender,
                     HireDate = e.HireDate,
                     DepartmentId = e.DepartmentId,
                     FacilityId = e.FacilityId,
                     JobTitle = e.JobTitle,
                     Email = e.Email,
                     PhoneNumber = e.PhoneNumber,
                     Address = e.Address,
                     Salary = e.Salary,
                     Status = e.Status,
                     ManagerId = e.ManagerId,
                     EmergencyContact = e.EmergencyContact,
                     Notes = e.Notes
                });
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetSalaryAboveAsync(decimal salary)
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                var filtered = employees.Where(e => e.Salary > salary).ToList();

                if (!filtered.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees with salary above the specified amount", 404);

                var dtos = filtered.Select(e => new EmployeeDTO
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes
                });
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetByManagerAsync(int managerId)
        {
            try
            {
                var employees = await _repository.GetByManagerAsync(managerId);
                if (employees == null || !employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees found under the specified manager", 404);

                var dtos = employees.Select(e => new EmployeeDTO
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes
                });
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetByStatusAsync(string status)
        {
            try
            {
                var employees = await _repository.GetByStatusAsync(status);
                if (employees == null || !employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees found with the specified status", 404);

                var dtos = employees.Select(e => new EmployeeDTO
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes
                });
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetBirthdaysThisMonthAsync()
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                var currentMonth = DateTime.UtcNow.Month;
                var result = await _repository.GetBirthdaysThisMonthAsync();

                if (!result.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees have birthdays this month", 404);

                var dtos = result.Select(e => new EmployeeDTO
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Gender = e.Gender,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId,
                    FacilityId = e.FacilityId,
                    JobTitle = e.JobTitle,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Salary = e.Salary,
                    Status = e.Status,
                    ManagerId = e.ManagerId,
                    EmergencyContact = e.EmergencyContact,
                    Notes = e.Notes
                });
                return Result<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeDTO>>.Failure($"Error.", 500);
            }
        }

        public async Task<Result<bool>> IncreaseSalaryHiredBeforeAsync(int years, decimal percentage)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddYears(-years);
                var employees = await _repository.GetByHireDateBeforeAsync(cutoffDate); 
                if (employees == null || !employees.Any())
                    return Result<bool>.Failure($"No employees hired more than {years} years ago.", 404);

                foreach (var employee in employees)
                {
                    employee.Salary += employee.Salary * (percentage / 100);
                    await _repository.UpdateAsync(employee);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to increase salaries: {ex.Message}", 500);
            }
        } 
    }
}
