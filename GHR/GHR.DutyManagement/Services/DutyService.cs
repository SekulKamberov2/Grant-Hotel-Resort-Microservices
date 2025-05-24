namespace GHR.DutyManagement.Services
{
    using GHR.DutyManagement.DTOs;
    using GHR.DutyManagement.Entities;
    using GHR.DutyManagement.Repositories;
    using GHR.SharedKernel;
    public interface IDutyService
    {
        Task<IdentityResult<IEnumerable<Duty>>> GetAllDutiesAsync();
        Task<IdentityResult<Duty>> GetDutyByIdAsync(int id);
        Task<IdentityResult<int>> CreateDutyAsync(DutyDTO duty);
        Task<IdentityResult<bool>> UpdateDutyAsync(Duty duty);
        Task<IdentityResult<bool>> DeleteDutyAsync(int id);
        Task<IdentityResult<IEnumerable<Shift>>> GetAllShiftsAsync();
        Task<IdentityResult<IEnumerable<PeriodType>>> GetAllPeriodTypesAsync();
        Task<IdentityResult<IEnumerable<DutyAssignment>>> GetDutyAssignmentsAsync(int dutyId);
        Task<IdentityResult<int>> AssignDutyAsync(DutyAssignmentDTO dutyAssignment);
    }

    public class DutyService : IDutyService
    {
        private readonly IDutyRepository _repository;
        public DutyService(IDutyRepository repository) => _repository = repository;

        public async Task<IdentityResult<int>> AssignDutyAsync(DutyAssignmentDTO dutyAssignment)
        {
            try
            {
                var dutyAssignmentId = await _repository.AssignDutyAsync(dutyAssignment);
                return IdentityResult<int>.Success(dutyAssignmentId);
            }
            catch (Exception ex)
            {
                return IdentityResult<int>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<int>> CreateDutyAsync(DutyDTO duty)
        {
            try
            {
                var dutyId = await _repository.CreateDutyAsync(duty);
                return IdentityResult<int>.Success(dutyId);
            }
            catch (Exception ex)
            {
                return IdentityResult<int>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<bool>> DeleteDutyAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteDutyAsync(id);
                return deleted ? IdentityResult<bool>.Success(true) : IdentityResult<bool>.Failure("Delete failed.");
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<IEnumerable<Duty>>> GetAllDutiesAsync()
        {
            try
            {
                var duties = await _repository.GetAllDutiesAsync();
                return IdentityResult<IEnumerable<Duty>>.Success(duties);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<Duty>>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<IEnumerable<PeriodType>>> GetAllPeriodTypesAsync()
        {
            try
            {
                var periodTypes = await _repository.GetAllPeriodTypesAsync();
                return IdentityResult<IEnumerable<PeriodType>>.Success(periodTypes);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<PeriodType>>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<IEnumerable<Shift>>> GetAllShiftsAsync()
        {
            try
            {
                var shifts = await _repository.GetAllShiftsAsync();
                return IdentityResult<IEnumerable<Shift>>.Success(shifts);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<Shift>>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<IEnumerable<DutyAssignment>>> GetDutyAssignmentsAsync(int dutyId)
        {
            try
            {
                var assignments = await _repository.GetDutyAssignmentsAsync(dutyId);
                return IdentityResult<IEnumerable<DutyAssignment>>.Success(assignments);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<DutyAssignment>>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<Duty>> GetDutyByIdAsync(int id)
        {
            try
            {
                var duty = await _repository.GetDutyByIdAsync(id);
                return duty != null ? IdentityResult<Duty>.Success(duty) : IdentityResult<Duty>.Failure("Duty not found.");
            }
            catch (Exception ex)
            {
                return IdentityResult<Duty>.Failure(ex.Message);
            }
        }

        public async Task<IdentityResult<bool>> UpdateDutyAsync(Duty duty)
        {
            try
            {
                var updated = await _repository.UpdateDutyAsync(duty);
                return updated ? IdentityResult<bool>.Success(true) : IdentityResult<bool>.Failure("Update failed.");
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure(ex.Message);
            }
        }
    }
}
