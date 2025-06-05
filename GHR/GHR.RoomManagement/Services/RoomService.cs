 namespace GHR.RoomManagement.Services
{ 
    using GHR.RoomManagement.DTOs;
    using GHR.RoomManagement.Entities;
    using GHR.RoomManagement.Repositories;
    using GHR.SharedKernel; 

    public interface IRoomService
    {
        Task<Result<IEnumerable<Room>>> GetRoomsAsync(string? status, int? floor, string? type);
        Task<Result<Room?>> GetRoomByIdAsync(int id);
        Task<Result<int>> CreateRoomAsync(Room room);
        Task<Result<bool>> UpdateRoomAsync(int id, Room room);
        Task<Result<bool>> DeleteRoomAsync(int id);


        Task<Result<IEnumerable<RoomType>>> GetAllRoomTypesAsync();
        Task<Result<int>> CreateRoomTypeAsync(CreateRoomTypeDTO dto);
        Task<Result<bool>> UpdateRoomTypeAsync(UpdateRoomTypeDTO dto);
        Task<Result<bool>> DeleteRoomTypeAsync(int id); 
        Task<Result<IEnumerable<RoomAvailabilityDTO>>> GetAvailableRoomsAsync(DateTime? start, DateTime? end, string? type);
        Task<Result<RoomAvailabilityDTO?>> GetAvailabilityByRoomIdAsync(int roomId);
        Task<Result<IEnumerable<DTOs.DutyDTO?>>> GetAllHouseKeepingAsync(string facility, string status);
    }

    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomsRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        public RoomService(IRoomRepository roomsRepository, IHttpClientFactory httpClientFactory)
        {
            _roomsRepository = roomsRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result<IEnumerable<Room>>> GetRoomsAsync(string? status, int? floor, string? type)
        {
            try
            {
                var rooms = await _roomsRepository.GetAllAsync(status, floor, type);
                return Result<IEnumerable<Room>>.Success(rooms);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Room>>.Failure($"Failed to retrieve rooms. {ex.Message}", 500);
            }
        }

        public async Task<Result<Room?>> GetRoomByIdAsync(int id)
        {
            try
            {
                var room = await _roomsRepository.GetByIdAsync(id);
                if (room == null)
                    return Result<Room?>.Failure("Room not found", 404);

                return Result<Room?>.Success(room);
            }
            catch (Exception ex)
            {
                return Result<Room?>.Failure($"Failed to retrieve room. {ex.Message}", 500);
            }
        }

        public async Task<Result<int>> CreateRoomAsync(Room room)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(room.RoomNumber))
                    return Result<int>.Failure("Room number is required", 400);
             
                var newId = await _roomsRepository.CreateAsync(room);
          
                Console.WriteLine(newId);
                if (newId == 0)
                    return Result<int>.Failure("Failed to create a room", 400); 
                return Result<int>.Success(newId);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Failed to create a room. {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> UpdateRoomAsync(int id, Room room)
        {
            try
            {
                if (id != room.Id)
                    return Result<bool>.Failure("Id mismatch", 400);

                var updated = await _roomsRepository.UpdateAsync(id, room);
                if (!updated)
                    return Result<bool>.Failure("Room not found or update failed", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to update room. {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> DeleteRoomAsync(int id)
        {
            try
            {
                var deleted = await _roomsRepository.DeleteAsync(id);
                if (!deleted)
                    return Result<bool>.Failure("Room not found", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to delete room. {ex.Message}", 500);
            }
        }
         
        public async Task<Result<IEnumerable<RoomType>>> GetAllRoomTypesAsync()
        {
            try
            {
                var data = await _roomsRepository.GetAllRoomTypesAsync();
                return Result<IEnumerable<RoomType>>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RoomType>>.Failure($"Error loading room types. {ex.Message}", 500);
            }
        }

        public async Task<Result<int>> CreateRoomTypeAsync(CreateRoomTypeDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<int>.Failure("Name is required", 400);

                var id = await _roomsRepository.CreateRoomTypeAsync(new RoomType
                {
                    Name = dto.Name,
                    Description = dto.Description
                });
                if (id == 0)
                    return Result<int>.Failure("Failed to create a room type", 400);

                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Error creating room type. {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> UpdateRoomTypeAsync(UpdateRoomTypeDTO dto)
        {
            try
            {
                var updated = await _roomsRepository.UpdateRoomTypeAsync(new RoomType
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description
                });

                if (!updated)
                    return Result<bool>.Failure("Room type not found or update failed", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error updating room type. {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> DeleteRoomTypeAsync(int id)
        {
            try
            {
                var deleted = await _roomsRepository.DeleteRoomTypeAsync(id);
                if (!deleted)
                    return Result<bool>.Failure("Room type not found", 404);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting room type. {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<RoomAvailabilityDTO>>> GetAvailableRoomsAsync(DateTime? start, DateTime? end, string? type)
        { 
            if (!start.HasValue || !end.HasValue) 
                return Result<IEnumerable<RoomAvailabilityDTO>>.Failure("Start and end dates are required.", 400); 

            if (start > end) 
                return Result<IEnumerable<RoomAvailabilityDTO>>.Failure("Start date must be before end date.", 400); 

            try
            {
                var data = await _roomsRepository.GetAllAvailableRoomsAsync(start, end, type);  
                return Result<IEnumerable<RoomAvailabilityDTO>>.Success(data);
            }
            catch (Exception ex)
            { 
                return Result<IEnumerable<RoomAvailabilityDTO>>.Failure($"An error occurred while retrieving room availability: {ex.Message}", 500);
            }
        } 

        public async Task<Result<RoomAvailabilityDTO?>> GetAvailabilityByRoomIdAsync(int roomId)
        { 
            if (roomId <= 0) 
                return Result<RoomAvailabilityDTO?>.Failure("Invalid room ID provided.", 400);  
            try
            {
                var data = await _roomsRepository.GetRoomAvailabilityByIdAsync(roomId); 
                if (data == null) 
                    return Result<RoomAvailabilityDTO?>.Failure($"No availability found for room with ID {roomId}.", 404); 

                return Result<RoomAvailabilityDTO?>.Success(data);
            }
            catch (Exception ex)
            { 
                return Result<RoomAvailabilityDTO?>.Failure($"An unexpected error occurred while retrieving room availability: {ex.Message}", 500);
            }
        }

        public async Task<Result<IEnumerable<DutyDTO?>>> GetAllHouseKeepingAsync(string facility, string status)
        { 
            if (string.IsNullOrWhiteSpace(facility)) 
                return Result<IEnumerable<DutyDTO?>>.Failure("Facility parameter is required.");  

            if (string.IsNullOrWhiteSpace(status))
                return Result<IEnumerable<DutyDTO?>>.Failure($"Status parameter is invalid. Allowed values.");
               
            var client = _httpClientFactory.CreateClient("DutyServiceClient");   
            var url = $"/api/duties/housekeeping/facility/{facility}/status/{status}"; 
            var response = await client.GetFromJsonAsync<Result<IEnumerable<Duty>>>(url);

            var result = response?.Data?.Select(r => new DutyDTO
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                AssignedToUserId = r.AssignedToUserId,
                AssignedByUserId = r.AssignedByUserId,
                RoleRequired = r.RoleRequired,
                Facility = r.Facility,
                Status = r.Status,
                Priority = r.Priority,
                DueDate = r.DueDate,
                CompletionDate = r.CompletionDate,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt

            }) ?? Enumerable.Empty<DTOs.DutyDTO>(); 
            return Result<IEnumerable<DTOs.DutyDTO?>>.Success(result); 
        }


    }

}
