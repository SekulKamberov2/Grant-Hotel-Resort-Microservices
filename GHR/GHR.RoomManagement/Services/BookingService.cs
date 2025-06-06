namespace GHR.RoomManagement.Services
{
    using System.Net.Http.Json;
    using System.Text.Json;

    using MassTransit;

    using GHR.RoomManagement.DTOs;
    using GHR.RoomManagement.Entities; 
    using GHR.RoomManagement.Repositories;
    using GHR.SharedKernel;
    using GHR.SharedKernel.Models;
    using GHR.SharedKernel.Events;
    public interface IBookingService
    {
        Task<Result<IEnumerable<Reservation>>> GetAllReservationsAsync();
        Task<Result<Reservation?>> GetReservationByIdAsync(int id);
        Task<Result<int>> CreateReservationAsync(CreateReservationDTO dto);
        Task<Result<bool>> UpdateReservationAsync(int id, UpdateReservationDTO dto);
        Task<Result<bool>> DeleteReservationAsync(int id); 
        Task<Result<IEnumerable<RoomRate>>> GetAllRoomRatesAsync();
        Task<Result<RoomRate?>> GetRoomRateByIdAsync(int id);
        Task<Result<int>> CreateRoomRateAsync(CreateRoomRateDto dto);
        Task<Result<bool>> UpdateRoomRateAsync(int id, UpdateRoomRateDto dto);
        Task<Result<bool>> DeleteRoomRateAsync(int id); 
        Task<Result<bool>> CheckInAsync(int reservationId, int employeeId);
        Task<Result<bool>> CheckOutAsync(int reservationId, int employeeId);
    }

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpClientFactory _httpClientFactory;
        public BookingService(
            IBookingRepository bookingRepository, 
            IPublishEndpoint publishEndpoint,
            IHttpClientFactory httpClientFactory)
        {
            _bookingRepository = bookingRepository;
            _publishEndpoint = publishEndpoint;
            _httpClientFactory = httpClientFactory;
        }  

        public async Task<Result<IEnumerable<Reservation>>> GetAllReservationsAsync()
        {
            try
            {
                var data = await _bookingRepository.GetAllReservationsAsync();
                if (data == null)
                    return Result<IEnumerable<Reservation>>.Failure("No reservations found.", 404);

                var reservationList = data.ToList();
                if (!reservationList.Any())
                    return Result<IEnumerable<Reservation>>.Failure("No reservations available.", 204);

                return Result<IEnumerable<Reservation>>.Success(reservationList);
            }
            catch (TimeoutException timeoutEx)
            {
                return Result<IEnumerable<Reservation>>.Failure($"Request timed out: {timeoutEx.Message}", 504);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Reservation>>.Failure($"Unexpected error occurred: {ex.Message}", 500);
            }
        }

        public async Task<Result<Reservation?>> GetReservationByIdAsync(int id)
        {
            try
            {
                var data = await _bookingRepository.GetReservationByIdAsync(id); 
                return data is null
                    ? Result<Reservation?>.Failure("Reservation not found", 404)
                    : Result<Reservation?>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<Reservation?>.Failure($"Failed to load reservation: {ex.Message}", 500);
            }
        }

        public async Task<Result<int>> CreateReservationAsync(CreateReservationDTO dto) // TO DO: model validation
        {
            try
            {   
                var guest = new CreateUser(dto.Username, dto.Email, dto.Password, dto.PhoneNumber, 6); ; //6 is roleId HOTEL GUEST
              
                var client = _httpClientFactory.CreateClient("CreateUserClient"); 
                var response = await client.PostAsJsonAsync($"/api/users/signup", guest); 
                if (!response.IsSuccessStatusCode) 
                    return Result<int>.Failure($"Failed to create reservation.", 500);
               
                var user = await response.Content.ReadFromJsonAsync<CreateUserResponse>();
              
                if (user == null)
                    return Result<int>.Failure("User data not found in response.", 500);
               
                var reservation = new Reservation
                {
                    GuestId = user.Id,
                    RoomId = dto.RoomId,
                    CheckInDate = dto.CheckInDate,
                    CheckOutDate = dto.CheckOutDate,
                    Status = "Pending"
                };
                var newId = await _bookingRepository.CreateReservationAsync(reservation);
                return Result<int>.Success(newId);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Failed to create reservation: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> UpdateReservationAsync(int id, UpdateReservationDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return Result<bool>.Failure("ID mismatch", 400);

                var updated = await _bookingRepository.UpdateReservationAsync(new Reservation
                {
                    Id = dto.Id,
                    CheckInDate = dto.CheckInDate,
                    CheckOutDate = dto.CheckOutDate,
                    Status = dto.Status
                });

                return updated
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Reservation not found", 404);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to update reservation: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> DeleteReservationAsync(int id)
        {
            try
            {
                var deleted = await _bookingRepository.DeleteReservationAsync(id);
                return deleted
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Reservation not found", 404);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to delete reservation: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<RoomRate>>> GetAllRoomRatesAsync()
        {
            try
            {
                var data = await _bookingRepository.GetAllRoomRatesAsync();
                return data.Any()
                    ? Result<IEnumerable<RoomRate>>.Success(data)
                    : Result<IEnumerable<RoomRate>>.Failure("No rates found", 204);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RoomRate>>.Failure($"Error loading rates: {ex.Message}", 500);
            }
        }

        public async Task<Result<RoomRate?>> GetRoomRateByIdAsync(int id)
        {
            try
            {
                var rate = await _bookingRepository.GetRoomRateByIdAsync(id);
                return rate != null
                    ? Result<RoomRate?>.Success(rate)
                    : Result<RoomRate?>.Failure("Rate not found", 404);
            }
            catch (Exception ex)
            {
                return Result<RoomRate?>.Failure($"Error retrieving rate: {ex.Message}", 500);
            }
        }

        public async Task<Result<int>> CreateRoomRateAsync(CreateRoomRateDto dto)
        {
            try
            {
                if (dto.PricePerNight <= 0)
                    return Result<int>.Failure("Price must be greater than 0", 400);

                var rate = new RoomRate
                {
                    RoomTypeId = dto.RoomTypeId,
                    PricePerNight = dto.PricePerNight,
                    ValidFrom = dto.ValidFrom,
                    ValidTo = dto.ValidTo
                };

                int newId = await _bookingRepository.CreateRoomRateAsync(rate);
                return Result<int>.Success(newId);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Error creating rate: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> UpdateRoomRateAsync(int id, UpdateRoomRateDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return Result<bool>.Failure("ID mismatch", 400);

                var rate = new RoomRate
                {
                    Id = dto.Id,
                    RoomTypeId = dto.RoomTypeId,
                    PricePerNight = dto.PricePerNight,
                    ValidFrom = dto.ValidFrom,
                    ValidTo = dto.ValidTo
                };

                bool updated = await _bookingRepository.UpdateRoomRateAsync(rate);
                return updated
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Rate not found or update failed", 404);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error updating rate: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> DeleteRoomRateAsync(int id)
        {
            try
            {
                bool deleted = await _bookingRepository.DeleteRoomRateAsync(id);
                return deleted
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Rate not found", 404);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting rate: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> CheckInAsync(int reservationId, int employeeId)
        {
            try
            {
                if (reservationId <= 0 || employeeId <= 0)
                    return Result<bool>.Failure("Invalid input.", 400);

                bool result = await _bookingRepository.CheckInAsync(reservationId, employeeId);
                return result
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure("Check-in failed or reservation not found.", 404);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error during check-in: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> CheckOutAsync(int reservationId, int employeeId)
        {
            try
            {
                if (reservationId <= 0 || employeeId <= 0)
                    return Result<bool>.Failure("Invalid input.", 400);

                bool result = await _bookingRepository.CheckOutAsync(reservationId, employeeId); 
                if (result)
                {
                    var evt = new CheckOutCompletedEvent
                    {
                        Facility = "HOTEL ROOM",
                        CheckInTime = DateTime.UtcNow
                    }; 
                    await _publishEndpoint.Publish(evt); 
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Check-in failed or reservation not found.", 404);

            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error during check-out: {ex.Message}", 500);
            }
        }
    }
}
