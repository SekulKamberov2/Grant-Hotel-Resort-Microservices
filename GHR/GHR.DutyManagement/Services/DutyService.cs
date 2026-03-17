namespace GHR.DutyManagement.Services
{
    using GHR.DutyManagement.DTOs;
    using GHR.DutyManagement.Entities;
    using GHR.DutyManagement.Repositories;
    using GHR.SharedKernel; 

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
        private readonly ILogger<DutyService> _logger;

        public DutyService(IDutyRepository repository, ILogger<DutyService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Duty>>> GetAllDutiesAsync()
        {
            try
            {
                var duties = await _repository.GetAllDutiesAsync();
                return Result<IEnumerable<Duty>>.Success(duties ?? Enumerable.Empty<Duty>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all duties");
                return Result<IEnumerable<Duty>>.Failure("An error occurred while retrieving duties.", 500);
            }
        }

        public async Task<Result<Duty>> GetDutyByIdAsync(int id)
        {
            try
            {
                var duty = await _repository.GetDutyByIdAsync(id);
                if (duty == null)
                    return Result<Duty>.Failure("Duty not found.", 404);
                return Result<Duty>.Success(duty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching duty by ID {Id}", id);
                return Result<Duty>.Failure("Error retrieving duty.", 500);
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
                _logger.LogError(ex, "Error creating duty");
                return Result<int>.Failure("Failed to create duty.", 500);
            }
        }

        public async Task<Result<bool>> UpdateDutyAsync(Duty duty)
        {
            try
            {
                var updated = await _repository.UpdateDutyAsync(duty);
                if (!updated)
                    return Result<bool>.Failure("Duty not found or update failed.", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating duty {Id}", duty.Id);
                return Result<bool>.Failure("Failed to update duty.", 500);
            }
        }

        public async Task<Result<bool>> DeleteDutyAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteDutyAsync(id);
                if (!deleted)
                    return Result<bool>.Failure("Duty not found.", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting duty {Id}", id);
                return Result<bool>.Failure("Failed to delete duty.", 500);
            }
        }

        public async Task<Result<IEnumerable<Shift>>> GetAllShiftsAsync()
        {
            try
            {
                var shifts = await _repository.GetAllShiftsAsync();
                return Result<IEnumerable<Shift>>.Success(shifts ?? Enumerable.Empty<Shift>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all shifts");
                return Result<IEnumerable<Shift>>.Failure("Error retrieving shifts.", 500);
            }
        }

        public async Task<Result<IEnumerable<PeriodType>>> GetAllPeriodTypesAsync()
        {
            try
            {
                var periodTypes = await _repository.GetAllPeriodTypesAsync();
                return Result<IEnumerable<PeriodType>>.Success(periodTypes ?? Enumerable.Empty<PeriodType>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all period types");
                return Result<IEnumerable<PeriodType>>.Failure("Error retrieving period types.", 500);
            }
        }

        public async Task<Result<IEnumerable<DutyAssignment>>> GetDutyAssignmentsAsync(int dutyId)
        {
            try
            {
                var assignments = await _repository.GetDutyAssignmentsAsync(dutyId);
                return Result<IEnumerable<DutyAssignment>>.Success(assignments ?? Enumerable.Empty<DutyAssignment>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching assignments for duty {DutyId}", dutyId);
                return Result<IEnumerable<DutyAssignment>>.Failure("Error retrieving duty assignments.", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeIdManagerIdDTO>>> GetAvailableStaffAsync(string facility)
        {
            try
            {
                var staff = await _repository.GetAvailableStaffAsync(facility);
                return Result<IEnumerable<EmployeeIdManagerIdDTO>>.Success(staff ?? Enumerable.Empty<EmployeeIdManagerIdDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available staff for facility '{Facility}'", facility);
                return Result<IEnumerable<EmployeeIdManagerIdDTO>>.Failure("Error retrieving available staff.", 500);
            }
        }

        public async Task<Result<IEnumerable<Duty>>> GetByFacilityAndStatusAsync(string facility, string status)
        {
            try
            {
                var duties = await _repository.GetByFacilityAndStatusAsync(facility, status);
                return Result<IEnumerable<Duty>>.Success(duties ?? Enumerable.Empty<Duty>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching duties for facility '{Facility}' and status '{Status}'", facility, status);
                return Result<IEnumerable<Duty>>.Failure("Error retrieving duties.", 500);
            }
        }

        public async Task<Result<int>> AssignDutyAsync(DutyAssignmentDTO dutyAssignment)
        {
            try
            {
                var id = await _repository.AssignDutyAsync(dutyAssignment);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning duty");
                return Result<int>.Failure("Failed to assign duty.", 500);
            }
        }
    }
}
