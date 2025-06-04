namespace GHR.DutyManagement.Services
{
    using GHR.DutyManagement.DTOs;
    using GHR.DutyManagement.Entities;
    using GHR.DutyManagement.Repositories;
    using GHR.SharedKernel;
    using System.Text.Json;

    public interface IDutyService
    {
        Task<Result<IEnumerable<Duty>>> GetAllDutiesAsync();
        Task<Result<Duty>> GetDutyByIdAsync(int id);
        Task<Result<int>> CreateDutyAsync(DutyDTO duty);
        Task<Result<bool>> UpdateDutyAsync(Duty duty);
        Task<Result<bool>> DeleteDutyAsync(int id);
        Task<Result<IEnumerable<Shift>>> GetAllShiftsAsync();
        Task<Result<IEnumerable<PeriodType>>> GetAllPeriodTypesAsync();
        Task<Result<IEnumerable<DutyAssignment>>> GetDutyAssignmentsAsync(int dutyId);
        Task<Result<IEnumerable<EmployeeIdManagerIdDTO>>> GetAvailableStaffAsync(string facility);
        Task<Result<IEnumerable<Duty>>> GetByFacilityAndStatusAsync(string facility, string status);
        Task<Result<int>> AssignDutyAsync(DutyAssignmentDTO dutyAssignment);
    }

    public class DutyService : IDutyService
    {
        private readonly IDutyRepository _repository;
        public DutyService(IDutyRepository repository) => _repository = repository;

        public async Task<Result<IEnumerable<Duty>>> GetByFacilityAndStatusAsync(string facility, string status)
        {
            try
            {
                var duties = await _repository.GetByFacilityAndStatusAsync(facility, status);
                return Result<IEnumerable<Duty>>.Success(duties);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Duty>>.Failure(ex.Message);
            }
        }

        public async Task<Result<int>> AssignDutyAsync(DutyAssignmentDTO dutyAssignment)
        {
            try
            {
                var dutyAssignmentId = await _repository.AssignDutyAsync(dutyAssignment);
                return Result<int>.Success(dutyAssignmentId);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex.Message);
            }
        }

        public async Task<Result<int>> CreateDutyAsync(DutyDTO duty)
        {
            try
            {
                var dutyId = await _repository.CreateDutyAsync(duty);
                return Result<int>.Success(dutyId);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteDutyAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteDutyAsync(id);
                return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete failed.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<Duty>>> GetAllDutiesAsync()
        {
            try
            {
                var duties = await _repository.GetAllDutiesAsync();
                return Result<IEnumerable<Duty>>.Success(duties);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Duty>>.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<PeriodType>>> GetAllPeriodTypesAsync()
        {
            try
            {
                var periodTypes = await _repository.GetAllPeriodTypesAsync();
                return Result<IEnumerable<PeriodType>>.Success(periodTypes);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<PeriodType>>.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<Shift>>> GetAllShiftsAsync()
        {
            try
            {
                var shifts = await _repository.GetAllShiftsAsync();
                return Result<IEnumerable<Shift>>.Success(shifts);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Shift>>.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<DutyAssignment>>> GetDutyAssignmentsAsync(int dutyId)
        {
            try
            {
                var assignments = await _repository.GetDutyAssignmentsAsync(dutyId);
                return Result<IEnumerable<DutyAssignment>>.Success(assignments);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DutyAssignment>>.Failure(ex.Message);
            }
        } 
        public async Task<Result<IEnumerable<EmployeeIdManagerIdDTO>>> GetAvailableStaffAsync(string facility)
        {
            try
            { 
                var assignments = await _repository.GetAvailableStaffAsync(facility); 
    
                return Result<IEnumerable<EmployeeIdManagerIdDTO>>.Success(assignments);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<EmployeeIdManagerIdDTO>>.Failure(ex.Message);
            }
        } 

        public async Task<Result<Duty>> GetDutyByIdAsync(int id)
        {
            try
            {
                var duty = await _repository.GetDutyByIdAsync(id);
                return duty != null ? Result<Duty>.Success(duty) : Result<Duty>.Failure("Duty not found.");
            }
            catch (Exception ex)
            {
                return Result<Duty>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateDutyAsync(Duty duty)
        {
            try
            {
                var updated = await _repository.UpdateDutyAsync(duty);
                return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update failed.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
