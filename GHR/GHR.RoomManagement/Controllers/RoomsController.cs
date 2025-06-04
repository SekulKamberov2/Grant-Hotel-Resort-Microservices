namespace GHR.RoomManagement.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using GHR.RoomManagement.DTOs;
    using GHR.RoomManagement.Entities;
    using GHR.RoomManagement.Services; 
    public class RoomsController : BaseApiController
    {
        private readonly IRoomService _roomManagementService; 
        public RoomsController(IRoomService roomManagementService) => _roomManagementService = roomManagementService;

        [HttpGet]
        public async Task<IActionResult> GetRooms([FromQuery] int? floor, [FromQuery] string? type, [FromQuery] string? status) =>
            AsActionResult(await _roomManagementService.GetRoomsAsync(status, floor, type)); 

        [HttpGet("get-room/{id}")]
        public async Task<IActionResult> GetRoom(int id) =>
            AsActionResult(await _roomManagementService.GetRoomByIdAsync(id));  

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] Room room) =>
            AsActionResult(await _roomManagementService.CreateRoomAsync(room));   

        [HttpPut("update-room/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
        {
            if (id != room.Id) return BadRequest("Id mismatch"); 
            return AsActionResult(await _roomManagementService.UpdateRoomAsync(id, room));
        }

        [HttpDelete("delete-room/{id}")]
        public async Task<IActionResult> DeleteRoom(int id) =>
            AsActionResult(await _roomManagementService.DeleteRoomAsync(id));

        [HttpGet("room-types")]
        public async Task<IActionResult> GetAllRoomTypes() =>
            AsActionResult(await _roomManagementService.GetAllRoomTypesAsync());  

        [HttpPost("create-room-type")]
        public async Task<IActionResult> CreateRoomType([FromBody] CreateRoomTypeDTO dto) =>
            AsActionResult(await _roomManagementService.CreateRoomTypeAsync(dto));  

        [HttpPut("update-room-type/{id}")]
        public async Task<IActionResult> UpdateRoomType(int id, [FromBody] UpdateRoomTypeDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            return AsActionResult(await _roomManagementService.UpdateRoomTypeAsync(dto));
        } 

        [HttpDelete("delete-room-type/{id}")]
        public async Task<IActionResult> DeleteRoomType(int id) =>
            AsActionResult(await _roomManagementService.DeleteRoomTypeAsync(id));

        // GET http://localhost:5093/api/rooms/availability-all?startDate=2025-06-10&endDate=2025-06-15&type=Deluxe

        [HttpGet("availability-all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate, 
            [FromQuery] string? type) =>
            AsActionResult(await _roomManagementService.GetAvailableRoomsAsync(startDate, endDate, type)); 

        // GET /api/availability/{roomId}
        [HttpGet("availability-room/{roomId}")]
        public async Task<IActionResult> GetByRoomId(int roomId) =>
            AsActionResult(await _roomManagementService.GetAvailabilityByRoomIdAsync(roomId));

        [HttpGet("housekeeping/facility/{facility}/status/{status}")]
        public async Task<IActionResult> GetAllHouseKeeping(string facility, string status) =>
            AsActionResult(await _roomManagementService.GetAllHouseKeepingAsync(facility, status));
    }
}
