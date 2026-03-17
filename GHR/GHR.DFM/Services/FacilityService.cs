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
        private readonly ILogger<FacilityService> _logger;

        public FacilityService(IFacilityRepository repository, ILogger<FacilityService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Facility>>> GetAllAsync()
        {
            try
            {
                var facilities = await _repository.GetAllAsync();
                return Result<IEnumerable<Facility>>.Success(facilities ?? Enumerable.Empty<Facility>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all facilities");
                return Result<IEnumerable<Facility>>.Failure("An error occurred while retrieving facilities.", 500);
            }
        }

        public async Task<Result<Facility?>> GetByIdAsync(int id)
        {
            try
            {
                var facility = await _repository.GetByIdAsync(id);
                if (facility == null)
                    return Result<Facility?>.Failure("Facility not found", 404);
                return Result<Facility?>.Success(facility);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching facility by ID {Id}", id);
                return Result<Facility?>.Failure("Error retrieving facility.", 500);
            }
        }

        public async Task<Result<int>> CreateAsync(Facility facility)
        {
            try
            {
                facility.CreatedAt = DateTime.UtcNow;
                var id = await _repository.CreateAsync(facility);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating facility");
                return Result<int>.Failure("Failed to create facility", 500);
            }
        }

        public async Task<Result<bool>> UpdateAsync(Facility facility)
        {
            try
            {
                facility.UpdatedAt = DateTime.UtcNow;
                var updated = await _repository.UpdateAsync(facility);
                if (!updated)
                    return Result<bool>.Failure("Facility not found or update failed", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating facility {Id}", facility.Id);
                return Result<bool>.Failure("Failed to update facility", 500);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                    return Result<bool>.Failure($"Facility with id {id} not found", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting facility {Id}", id);
                return Result<bool>.Failure("Failed to delete facility", 500);
            }
        }

        public async Task<Result<IEnumerable<string>>> GetFacilityTypesAsync()
        {
            try
            {
                var types = await _repository.GetFacilityTypesAsync();
                return Result<IEnumerable<string>>.Success(types ?? Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching facility types");
                return Result<IEnumerable<string>>.Failure("Error retrieving facility types", 500);
            }
        }

        public async Task<Result<IEnumerable<string>>> GetFacilityStatusesAsync()
        {
            try
            {
                var statuses = await _repository.GetFacilityStatusesAsync();
                return Result<IEnumerable<string>>.Success(statuses ?? Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching facility statuses");
                return Result<IEnumerable<string>>.Failure("Error retrieving facility statuses", 500);
            }
        }

        public async Task<Result<bool>> UpdateFacilityStatusAsync(int id, string status)
        {
            try
            {
                var updated = await _repository.UpdateFacilityStatusAsync(id, status);
                if (!updated)
                    return Result<bool>.Failure($"Facility with id {id} not found", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for facility {Id}", id);
                return Result<bool>.Failure("Failed to update facility status", 500);
            }
        }

        public async Task<Result<bool>> CreateFacilityScheduleAsync(FacilitySchedule schedule)
        {
            try
            {
                var success = await _repository.CreateFacilityScheduleAsync(schedule);
                return Result<bool>.Success(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating facility schedule for facility {FacilityId}", schedule.FacilityId);
                return Result<bool>.Failure("Failed to create facility schedule", 500);
            }
        }

        public async Task<Result<IEnumerable<Facility>>> GetAvailableFacilitiesAsync()
        {
            try
            {
                var facilities = await _repository.GetAvailableFacilitiesAsync();
                return Result<IEnumerable<Facility>>.Success(facilities ?? Enumerable.Empty<Facility>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available facilities");
                return Result<IEnumerable<Facility>>.Failure("Error retrieving available facilities", 500);
            }
        }

        public async Task<Result<IEnumerable<FacilitySchedule>>> GetFacilityScheduleAsync(int facilityId)
        {
            try
            {
                var schedules = await _repository.GetFacilityScheduleAsync(facilityId);
                return Result<IEnumerable<FacilitySchedule>>.Success(schedules ?? Enumerable.Empty<FacilitySchedule>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schedule for facility {FacilityId}", facilityId);
                return Result<IEnumerable<FacilitySchedule>>.Failure("Error retrieving facility schedule", 500);
            }
        }

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
                _logger.LogError(ex, "Error updating schedule for facility {FacilityId}", facilityId);
                return Result<bool>.Failure("Failed to update facility schedule", 500);
            }
        }

        public async Task<Result<IEnumerable<Facility>>> GetNearbyFacilitiesAsync(string location)
        {
            try
            {
                var facilities = await _repository.GetNearbyFacilitiesAsync(location);
                return Result<IEnumerable<Facility>>.Success(facilities ?? Enumerable.Empty<Facility>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching nearby facilities for location '{Location}'", location);
                return Result<IEnumerable<Facility>>.Failure("Error retrieving nearby facilities", 500);
            }
        }

        public async Task<Result<IEnumerable<FacilityServiceItem>>> GetFacilityServicesAsync(int facilityId)
        {
            try
            {
                var services = await _repository.GetFacilityServicesAsync(facilityId);
                return Result<IEnumerable<FacilityServiceItem>>.Success(services ?? Enumerable.Empty<FacilityServiceItem>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services for facility {FacilityId}", facilityId);
                return Result<IEnumerable<FacilityServiceItem>>.Failure("Error retrieving facility services", 500);
            }
        }

        public async Task<Result<int>> AddFacilityServiceAsync(FacilityServiceItem service)
        {
            try
            {
                var id = await _repository.AddFacilityServiceAsync(service);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding service for facility {FacilityId}", service.FacilityId);
                return Result<int>.Failure("Failed to add facility service", 500);
            }
        }

        public async Task<Result<bool>> DeleteFacilityServiceAsync(int facilityId, int serviceId)
        {
            try
            {
                var deleted = await _repository.DeleteFacilityServiceAsync(facilityId, serviceId);
                if (!deleted)
                    return Result<bool>.Failure($"Service {serviceId} not found for facility {facilityId}", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service {ServiceId} for facility {FacilityId}", serviceId, facilityId);
                return Result<bool>.Failure("Failed to delete facility service", 500);
            }
        }

        public async Task<Result<int>> CreateReservationAsync(FacilityReservation reservation)
        {
            try
            {
                reservation.CreatedAt = DateTime.UtcNow;
                var id = await _repository.CreateReservationAsync(reservation);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation for facility {FacilityId}", reservation.FacilityId);
                return Result<int>.Failure("Failed to create reservation", 500);
            }
        }

        public async Task<Result<IEnumerable<FacilityReservation>>> GetReservationsByFacilityAsync(int facilityId)
        {
            try
            {
                var reservations = await _repository.GetReservationsByFacilityAsync(facilityId);
                return Result<IEnumerable<FacilityReservation>>.Success(reservations ?? Enumerable.Empty<FacilityReservation>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservations for facility {FacilityId}", facilityId);
                return Result<IEnumerable<FacilityReservation>>.Failure("Error retrieving reservations", 500);
            }
        }

        public async Task<Result<bool>> DeleteReservationAsync(int facilityId, int reservationId)
        {
            try
            {
                var deleted = await _repository.DeleteReservationAsync(facilityId, reservationId);
                if (!deleted)
                    return Result<bool>.Failure($"Reservation {reservationId} not found for facility {facilityId}", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reservation {ReservationId} for facility {FacilityId}", reservationId, facilityId);
                return Result<bool>.Failure("Failed to delete reservation", 500);
            }
        }

        public async Task<Result<int>> ReportIssueAsync(FacilityIssue issue)
        {
            try
            {
                issue.ReportedAt = DateTime.UtcNow;
                var id = await _repository.ReportIssueAsync(issue);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting issue for facility {FacilityId}", issue.FacilityId);
                return Result<int>.Failure("Failed to report issue", 500);
            }
        }

        public async Task<Result<IEnumerable<FacilityIssue>>> GetOpenIssuesAsync(int facilityId)
        {
            try
            {
                var issues = await _repository.GetOpenIssuesAsync(facilityId);
                return Result<IEnumerable<FacilityIssue>>.Success(issues ?? Enumerable.Empty<FacilityIssue>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching open issues for facility {FacilityId}", facilityId);
                return Result<IEnumerable<FacilityIssue>>.Failure("Error retrieving issues", 500);
            }
        }

        public async Task<Result<bool>> AssignMaintenanceAsync(int facilityId, int issueId, string assignedTo)
        {
            try
            {
                var success = await _repository.AssignMaintenanceAsync(facilityId, issueId, assignedTo);
                if (!success)
                    return Result<bool>.Failure($"Issue {issueId} not found for facility {facilityId}", 404);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning maintenance for issue {IssueId} at facility {FacilityId}", issueId, facilityId);
                return Result<bool>.Failure("Failed to assign maintenance", 500);
            }
        }

        public async Task<Result<IEnumerable<FacilityReservation>>> GetUsageHistoryAsync(int facilityId)
        {
            try
            {
                var history = await _repository.GetUsageHistoryAsync(facilityId);
                return Result<IEnumerable<FacilityReservation>>.Success(history ?? Enumerable.Empty<FacilityReservation>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching usage history for facility {FacilityId}", facilityId);
                return Result<IEnumerable<FacilityReservation>>.Failure("Error retrieving usage history", 500);
            }
        }

        public async Task<Result<IEnumerable<TimeSpan>>> GetAvailableSlotsAsync(int facilityId, DateTime date)
        {
            try
            {
                var slots = await _repository.GetAvailableSlotsAsync(facilityId, date);
                return Result<IEnumerable<TimeSpan>>.Success(slots ?? Enumerable.Empty<TimeSpan>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available slots for facility {FacilityId} on {Date}", facilityId, date);
                return Result<IEnumerable<TimeSpan>>.Failure("Error retrieving available slots", 500);
            }
        }
    }
}
