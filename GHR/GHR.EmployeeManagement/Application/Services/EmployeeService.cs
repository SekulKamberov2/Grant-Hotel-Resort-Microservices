namespace GHR.EmployeeManagement.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Leaverequests.Grpc;

    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Domain.Entities;
    using GHR.EmployeeManagement.Infrastructure.Repositories;
    using GHR.SharedKernel;
 
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
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository repository,
            LeaverequestsService.LeaverequestsServiceClient leaveRequestsClient,
            ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _leaveRequestsClient = leaveRequestsClient;
            _logger = logger;
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
                _logger.LogError(ex, "Error fetching all employees");
                return Result<IEnumerable<EmployeeDTO>>.Failure("An error occurred while retrieving employees.", 500);
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
                _logger.LogError(ex, "Error fetching employee by ID {Id}", id);
                return Result<EmployeeDTO>.Failure("Error retrieving employee.", 500);
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
                _logger.LogError(ex, "Error fetching employee by UserId {UserId}", userId);
                return Result<EmployeeDTO>.Failure("Error retrieving employee.", 500);
            }
        }

        public async Task<Result<EmployeeWithAllLeaveRequestsDTO>> GetAllLeaveRequestsByUserIdAsync(int userId)
        {
            try
            {
                var employee = await _repository.GetByUserIdAsync(userId);
                if (employee == null)
                    return Result<EmployeeWithAllLeaveRequestsDTO>.Failure("Employee not found", 404);

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

                var request = new GetLeaveRequest { UserId = userId };
                var allLeaveRequests = await _leaveRequestsClient.GetLeaveRequestsByEmployeeAsync(request);

                if (allLeaveRequests != null)
                {
                    dto.AllLeaveRequests = allLeaveRequests.Leaves.Select(l => new LeaveRequestDTO
                    {
                        Department = l.Department,
                        LeaveTypeId = l.LeaveTypeId,
                        StartDate = string.IsNullOrEmpty(l.StartDate) ? null : DateTime.Parse(l.StartDate),
                        EndDate = string.IsNullOrEmpty(l.EndDate) ? null : DateTime.Parse(l.EndDate),
                        TotalDays = decimal.TryParse(l.TotalDays, out var totalDaysValue) ? totalDaysValue : null,
                        Reason = l.Reason,
                        ApproverId = l.ApproverId,
                        DecisionDate = string.IsNullOrEmpty(l.DecisionDate) ? null : DateTime.Parse(l.DecisionDate),
                        RequestedAt = string.IsNullOrEmpty(l.RequestedAt) ? null : DateTime.Parse(l.RequestedAt)
                    });
                }

                return Result<EmployeeWithAllLeaveRequestsDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave requests for user {UserId}", userId);
                return Result<EmployeeWithAllLeaveRequestsDTO>.Failure("Error retrieving leave requests.", 500);
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
                _logger.LogError(ex, "Error searching employees by name '{Name}'", name);
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error searching employees.", 500);
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
                _logger.LogError(ex, "Error fetching employees by department {DepartmentId}", departmentId);
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error retrieving employees.", 500);
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
                _logger.LogError(ex, "Error fetching employees by facility {FacilityId}", facilityId);
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error retrieving employees.", 500);
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
                return Result<Employee>.Success(createdEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return Result<Employee>.Failure("Failed to create employee", 500);
            }
        }

        public async Task<Result<Employee>> UpdateAsync(int id, UpdateEmployeeDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return Result<Employee>.Failure("ID mismatch", 400);

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
                return Result<Employee>.Success(updatedEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee {Id}", id);
                return Result<Employee>.Failure("Failed to update employee", 500);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(id);
                if (employee == null)
                    return Result<bool>.Failure("Employee not found", 404);

                await _repository.DeleteAsync(id); 
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee {Id}", id);
                return Result<bool>.Failure("Failed to delete employee", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetHiredAfterAsync(DateTime date)
        {
            try
            {
                var employees = await _repository.GetHiredAfterAsync(date);
                if (!employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees hired after the specified date", 404);

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
                _logger.LogError(ex, "Error fetching employees hired after {Date}", date);
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error retrieving employees", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetSalaryAboveAsync(decimal salary)
        {
            try
            {
                var employees = await _repository.GetWithSalaryAboveAsync(salary); 
                if (!employees.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees with salary above the specified amount", 404);

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
                _logger.LogError(ex, "Error fetching employees with salary above {Salary}", salary);
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error retrieving employees", 500);
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
                _logger.LogError(ex, "Error fetching employees by manager {ManagerId}", managerId);
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error retrieving employees", 500);
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
                _logger.LogError(ex, "Error fetching employees by status '{Status}'", status);
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error retrieving employees", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeDTO>>> GetBirthdaysThisMonthAsync()
        {
            try
            {
                var result = await _repository.GetBirthdaysThisMonthAsync();
                if (!result.Any())
                    return Result<IEnumerable<EmployeeDTO>>.Failure("No employees have birthdays this month", 404);

                var dtos = result.Select(e => new EmployeeDTO
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
                _logger.LogError(ex, "Error fetching birthdays this month");
                return Result<IEnumerable<EmployeeDTO>>.Failure("Error retrieving employees", 500);
            }
        }

        public async Task<Result<bool>> IncreaseSalaryHiredBeforeAsync(int years, decimal percentage)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddYears(-years);
                var rowsAffected = await _repository.IncreaseSalaryForHiredBeforeAsync(cutoffDate, percentage);

                if (rowsAffected == 0)
                    return Result<bool>.Failure($"No employees hired more than {years} years ago.", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error increasing salaries for employees hired before {CutoffDate}", DateTime.UtcNow.AddYears(-years));
                return Result<bool>.Failure("Failed to increase salaries", 500);
            }
        }
    }
}
