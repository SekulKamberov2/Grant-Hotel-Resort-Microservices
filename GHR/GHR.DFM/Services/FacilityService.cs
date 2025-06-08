namespace GHR.DFM.Services
{
    using GHR.DFM.Entities;
    using GHR.DFM.Repositories;
    using GHR.SharedKernel; 

    public interface IFacilityService
    {
        Task<Result<IEnumerable<Facility>>> GetAllAsync();
        Task<Result<Facility?>> GetByIdAsync(int id);
        Task<Result<int>> CreateAsync(Facility facility);
        Task<Result<bool>> UpdateAsync(Facility facility);
        Task<Result<bool>> DeleteAsync(int id); 
        Task<Result<IEnumerable<string>>> GetFacilityTypesAsync();
        Task<Result<IEnumerable<string>>> GetFacilityStatusesAsync();
        Task<Result<bool>> UpdateFacilityStatusAsync(int id, string status); 
        Task<Result<IEnumerable<Facility>>> GetAvailableFacilitiesAsync();
        Task<Result<IEnumerable<FacilitySchedule>>> GetFacilityScheduleAsync(int facilityId);
        Task<Result<bool>> UpdateFacilityScheduleAsync(int facilityId, IEnumerable<FacilitySchedule> schedules);  
        Task<Result<bool>> CreateFacilityScheduleAsync(FacilitySchedule schedule);  
        Task<Result<IEnumerable<Facility>>> GetNearbyFacilitiesAsync(string location);
        Task<Result<IEnumerable<FacilityServiceItem>>> GetFacilityServicesAsync(int facilityId);
        Task<Result<int>> AddFacilityServiceAsync(FacilityServiceItem service); 
        Task<Result<bool>> DeleteFacilityServiceAsync(int facilityId, int serviceId);
        Task<Result<int>> CreateReservationAsync(FacilityReservation reservation);
        Task<Result<IEnumerable<FacilityReservation>>> GetReservationsByFacilityAsync(int facilityId); 
        Task<Result<bool>> DeleteReservationAsync(int facilityId, int reservationId);
        Task<Result<int>> ReportIssueAsync(FacilityIssue issue);
        Task<Result<IEnumerable<FacilityIssue>>> GetOpenIssuesAsync(int facilityId);
        Task<Result<bool>> AssignMaintenanceAsync(int facilityId, int issueId, string assignedTo);
        public Task<Result<IEnumerable<FacilityReservation>>> GetUsageHistoryAsync(int facilityId);
        public Task<Result<IEnumerable<TimeSpan>>> GetAvailableSlotsAsync(int facilityId, DateTime date);
    }

    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _repository; 
        public FacilityService(IFacilityRepository repository) => _repository = repository;

        public async Task<Result<IEnumerable<Facility>>> GetAllAsync() =>
            Result<IEnumerable<Facility>>.Success(await _repository.GetAllAsync());
      
        public async Task<Result<Facility?>> GetByIdAsync(int id)
            => Result<Facility?>.Success(await _repository.GetByIdAsync(id));
         
        public async Task<Result<int>> CreateAsync(Facility facility)
        {
            facility.CreatedAt = DateTime.UtcNow;

            try
            {
                var newId = await _repository.CreateAsync(facility);
                return Result<int>.Success(newId);
            }
            catch (Exception ex)
            { 
                return Result<int>.Failure("Failed to create facility: " + ex.Message);
            }
        } 

        public async Task<Result<bool>> UpdateAsync(Facility facility)
        {
            facility.UpdatedAt = DateTime.UtcNow; 
            try
            {
                var updated = await _repository.UpdateAsync(facility);

                if (!updated)
                    return Result<bool>.Failure("Facility update failed or not found.", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            { 
                return Result<bool>.Failure("Exception during update: " + ex.Message);
            }
        } 

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);

                if (!deleted)
                    return Result<bool>.Failure($"Facility with id {id} not found or not deleted.", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting facility: {ex.Message}");
            }
        } 

        public async Task<Result<IEnumerable<string>>> GetFacilityTypesAsync()
        {
            var types = await _repository.GetFacilityTypesAsync();
            return Result<IEnumerable<string>>.Success(types ?? Enumerable.Empty<string>());
        } 

        public async Task<Result<IEnumerable<string>>> GetFacilityStatusesAsync() =>
            Result<IEnumerable<string>>.Success(await _repository.GetFacilityStatusesAsync()); 

        public async Task<Result<bool>> CreateFacilityScheduleAsync(FacilitySchedule schedule) =>
            Result<bool>.Success(await _repository.CreateFacilityScheduleAsync(schedule)); 

        public async Task<Result<bool>> UpdateFacilityStatusAsync(int id, string status)
        {
            try
            {
                var updated = await _repository.UpdateFacilityStatusAsync(id, status);
                if (!updated)
                    return Result<bool>.Failure($"Facility with id {id} not found or status update failed.", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error updating facility status: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Facility>>> GetAvailableFacilitiesAsync() =>
             Result<IEnumerable<Facility>>.Success(await _repository.GetAvailableFacilitiesAsync());
         
        public async Task<Result<IEnumerable<FacilitySchedule>>> GetFacilityScheduleAsync(int facilityId) =>
            Result<IEnumerable<FacilitySchedule>>.Success(await _repository.GetFacilityScheduleAsync(facilityId)); 

        public async Task<Result<bool>> UpdateFacilityScheduleAsync(int facilityId, IEnumerable<FacilitySchedule> schedules)
        {
            try
            {
                var updated = await _repository.UpdateFacilityScheduleAsync(facilityId, schedules);

                if (!updated)
                    return Result<bool>.Failure($"Failed to update schedule for facility {facilityId}", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error updating schedule for facility {facilityId}: {ex.Message}");
            }
        } 

        public async Task<Result<IEnumerable<Facility>>> GetNearbyFacilitiesAsync(string location) =>  
            Result<IEnumerable<Facility>>.Success(await _repository.GetNearbyFacilitiesAsync(location));
          
        public async Task<Result<IEnumerable<FacilityServiceItem>>> GetFacilityServicesAsync(int facilityId) =>
             Result<IEnumerable<FacilityServiceItem>>.Success(await _repository.GetFacilityServicesAsync(facilityId));
       
        public async Task<Result<int>> AddFacilityServiceAsync(FacilityServiceItem service)
        {
            try
            {
                var id = await _repository.AddFacilityServiceAsync(service);
                if (id <= 0)
                    return Result<int>.Failure("Failed to add new facility service.");

                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Error adding facility service: {ex.Message}");
            }
        } 
        public async Task<Result<bool>> DeleteFacilityServiceAsync(int facilityId, int serviceId)
        {
            try
            {
                var deleted = await _repository.DeleteFacilityServiceAsync(facilityId, serviceId);
                if (!deleted)
                    return Result<bool>.Failure($"Failed to delete service {serviceId} from facility {facilityId}.");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting facility service: {ex.Message}");
            }
        }  
        public async Task<Result<int>> CreateReservationAsync(FacilityReservation reservation)
        {
            reservation.CreatedAt = DateTime.UtcNow; 
            try
            {
                var id = await _repository.CreateReservationAsync(reservation);

                if (id <= 0)
                    return Result<int>.Failure("Failed to create reservation.");

                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Error creating reservation: {ex.Message}");
            }
        } 

        public async Task<Result<IEnumerable<FacilityReservation>>> GetReservationsByFacilityAsync(int facilityId) =>
            Result<IEnumerable<FacilityReservation>>.Success(await _repository.GetReservationsByFacilityAsync(facilityId));
        
        public async Task<Result<bool>> DeleteReservationAsync(int facilityId, int reservationId)
        {
            try
            {
                var deleted = await _repository.DeleteReservationAsync(facilityId, reservationId);

                if (!deleted)
                    return Result<bool>.Failure($"Failed to delete reservation {reservationId} for facility {facilityId}.");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting reservation: {ex.Message}");
            }
        }  

        public async Task<Result<int>> ReportIssueAsync(FacilityIssue issue)
        {
            issue.ReportedAt = DateTime.UtcNow; 
            try
            {
                var id = await _repository.ReportIssueAsync(issue);
                if (id <= 0)
                    return Result<int>.Failure("Failed to report issue.");

                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Error reporting issue: {ex.Message}");
            }
        } 

        public async Task<Result<IEnumerable<FacilityIssue>>> GetOpenIssuesAsync(int facilityId) =>
            Result<IEnumerable<FacilityIssue>>.Success(await _repository.GetOpenIssuesAsync(facilityId));
          
        public async Task<Result<bool>> AssignMaintenanceAsync(int facilityId, int issueId, string assignedTo)
        {
            try
            {
                var success = await _repository.AssignMaintenanceAsync(facilityId, issueId, assignedTo);
                if (!success)
                    return Result<bool>.Failure($"Failed to assign maintenance for issue {issueId} at facility {facilityId}.");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error assigning maintenance: {ex.Message}");
            }
        } 

        public async Task<Result<IEnumerable<FacilityReservation>>> GetUsageHistoryAsync(int facilityId) =>
            Result<IEnumerable<FacilityReservation>>.Success(await _repository.GetUsageHistoryAsync(facilityId));
          
        public async Task<Result<IEnumerable<TimeSpan>>> GetAvailableSlotsAsync(int facilityId, DateTime date) =>
            Result<IEnumerable<TimeSpan>>.Success(await _repository.GetAvailableSlotsAsync(facilityId, date));  
    }
}
