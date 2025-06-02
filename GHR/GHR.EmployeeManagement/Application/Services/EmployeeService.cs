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
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetAllAsync();
        Task<IdentityResult<EmployeeDTO>> GetByIdAsync(int id);
        Task<IdentityResult<EmployeeDTO>> GetByUserIdAsync(int userId);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> SearchByNameAsync(string name);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByDepartmentAsync(int departmentId);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByFacilityAsync(int facilityId);
        Task<IdentityResult<Employee>> CreateAsync(CreateEmployeeDTO dto);
        Task<IdentityResult<Employee>> UpdateAsync(int id, UpdateEmployeeDTO dto);
        Task<IdentityResult<bool>> DeleteAsync(int id);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetHiredAfterAsync(DateTime date);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetSalaryAboveAsync(decimal salary);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByManagerAsync(int managerId);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByStatusAsync(string status);
        Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetBirthdaysThisMonthAsync();
        Task<IdentityResult<bool>> IncreaseSalaryHiredBeforeAsync(int years, decimal percentage);
        Task<IdentityResult<EmployeeWithAllLeaveRequestsDTO>> GetAllLeaveRequestsByUserIdAsync(int userId);
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
         
        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetAllAsync()
        {
            try
            {
                var employees = await _repository.GetAllAsync(); 
                if (employees == null || !employees.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees found", 404);

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
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error.", 500);
            }
        }

        public async Task<IdentityResult<EmployeeDTO>> GetByIdAsync(int id)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(id); 
                if (employee == null)
                    return IdentityResult<EmployeeDTO>.Failure("Employee not found", 404);

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

                return IdentityResult<EmployeeDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return IdentityResult<EmployeeDTO>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<EmployeeDTO>> GetByUserIdAsync(int userId)
        {
            try
            {
                var employee = await _repository.GetByUserIdAsync(userId);
                if (employee == null)
                    return IdentityResult<EmployeeDTO>.Failure("Employee not found", 404);

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

                return IdentityResult<EmployeeDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return IdentityResult<EmployeeDTO>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<EmployeeWithAllLeaveRequestsDTO>> GetAllLeaveRequestsByUserIdAsync(int userId)
        {
            try
            { 
                var employee = await _repository.GetByUserIdAsync(userId);  
                if (employee == null)
                    return IdentityResult<EmployeeWithAllLeaveRequestsDTO>.Failure("Employee not found", 404);
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

                return IdentityResult<EmployeeWithAllLeaveRequestsDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return IdentityResult<EmployeeWithAllLeaveRequestsDTO>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> SearchByNameAsync(string name)
        {
            try
            { 
                var employees = await _repository.SearchByNameAsync(name);
                 
                if (employees == null || !employees.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees found with the given name", 404);
                 
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
                 
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            { 
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByDepartmentAsync(int departmentId)
        {
            try
            { 
                var employees = await _repository.GetByDepartmentAsync(departmentId);
                 
                if (employees == null || !employees.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees found in this department", 404);
                 
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
                 
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            { 
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error.", 500);
            }
        } 

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByFacilityAsync(int facilityId)
        {
            try
            {
                var employees = await _repository.GetByFacilityAsync(facilityId);

                if (employees == null || !employees.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees found for the given facility", 404);

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

                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error", 500);
            }
        } 
        public async Task<IdentityResult<Employee>> CreateAsync(CreateEmployeeDTO dto)
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
                    return IdentityResult<Employee>.Success(createdEmployee);

                return IdentityResult<Employee>.Failure("Failed to create employee");
            }
            catch (Exception ex)
            {
                return IdentityResult<Employee>.Failure($"Failed to create employee", 500);
            }
        }
         
        public async Task<IdentityResult<Employee>> UpdateAsync(int id, UpdateEmployeeDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return IdentityResult<Employee>.Failure("Employee not found", 404);

                var employee = await _repository.GetByIdAsync(id);
                if (employee == null)
                    return IdentityResult<Employee>.Failure("Employee not found", 404);

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
                    return IdentityResult<Employee>.Failure($"Failed to update employee.", 500);

                return IdentityResult<Employee>.Success(updatedEmployee);
            }
            catch (Exception ex)
            {
                return IdentityResult<Employee>.Failure($"Failed to update employee.", 500);
            }
        }  
        public async Task<IdentityResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(id);
                if (employee == null)
                    return IdentityResult<bool>.Failure("Employee not found", 404);

                await _repository.DeleteAsync(employee);
                return IdentityResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure($"Failed to delete employee.", 500);
            }
        }

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetHiredAfterAsync(DateTime date)
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                var filtered = employees.Where(e => e.HireDate >= date).ToList();

                if (!filtered.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees hired after the specified date", 404);

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
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetSalaryAboveAsync(decimal salary)
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                var filtered = employees.Where(e => e.Salary > salary).ToList();

                if (!filtered.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees with salary above the specified amount", 404);

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
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByManagerAsync(int managerId)
        {
            try
            {
                var employees = await _repository.GetByManagerAsync(managerId);
                if (employees == null || !employees.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees found under the specified manager", 404);

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
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetByStatusAsync(string status)
        {
            try
            {
                var employees = await _repository.GetByStatusAsync(status);
                if (employees == null || !employees.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees found with the specified status", 404);

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
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<IEnumerable<EmployeeDTO>>> GetBirthdaysThisMonthAsync()
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                var currentMonth = DateTime.UtcNow.Month;
                var result = await _repository.GetBirthdaysThisMonthAsync();

                if (!result.Any())
                    return IdentityResult<IEnumerable<EmployeeDTO>>.Failure("No employees have birthdays this month", 404);

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
                return IdentityResult<IEnumerable<EmployeeDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<EmployeeDTO>>.Failure($"Error.", 500);
            }
        }

        public async Task<IdentityResult<bool>> IncreaseSalaryHiredBeforeAsync(int years, decimal percentage)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddYears(-years);
                var employees = await _repository.GetByHireDateBeforeAsync(cutoffDate); 
                if (employees == null || !employees.Any())
                    return IdentityResult<bool>.Failure($"No employees hired more than {years} years ago.", 404);

                foreach (var employee in employees)
                {
                    employee.Salary += employee.Salary * (percentage / 100);
                    await _repository.UpdateAsync(employee);
                }

                return IdentityResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure($"Failed to increase salaries: {ex.Message}", 500);
            }
        } 
    }
}
