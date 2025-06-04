namespace GHR.RoomManagement.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using GHR.RoomManagement.DTOs;
    using GHR.RoomManagement.Services;
 
    public class BookingController : BaseApiController
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService) => _bookingService = bookingService;

        [HttpGet]
        public async Task<IActionResult> GetAllReservations() =>
            AsActionResult(await _bookingService.GetAllReservationsAsync());  

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) =>
            AsActionResult(await _bookingService.GetReservationByIdAsync(id));  

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDTO dto) =>
            AsActionResult(await _bookingService.CreateReservationAsync(dto));  

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDTO dto) =>
            AsActionResult(await _bookingService.UpdateReservationAsync(id, dto));   

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id) =>
            AsActionResult(await _bookingService.DeleteReservationAsync(id));

        [HttpGet]
        public async Task<IActionResult> GetAllRoomRates() =>
            AsActionResult(await _bookingService.GetAllRoomRatesAsync()); 

        [HttpGet("room-rate/{id}")]
        public async Task<IActionResult> GetRoomRateById(int id) =>
            AsActionResult(await _bookingService.GetRoomRateByIdAsync(id)); 

        [HttpPost]
        public async Task<IActionResult> CreateRoomRate([FromBody] CreateRoomRateDto dto) =>
            AsActionResult(await _bookingService.CreateRoomRateAsync(dto)); 

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoomRate(int id, [FromBody] UpdateRoomRateDto dto) =>
            AsActionResult(await _bookingService.UpdateRoomRateAsync(id, dto));  

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomRate(int id) =>
            AsActionResult(await _bookingService.DeleteRoomRateAsync(id));

        [HttpPost("checkin/{reservationId}")]
        public async Task<IActionResult> CheckIn(int reservationId, [FromBody] PerformActionDto dto) =>
            AsActionResult(await _bookingService.CheckInAsync(reservationId, dto.EmployeeId));
       
        [HttpPost("checkout/{reservationId}")]
        public async Task<IActionResult> CheckOut(int reservationId, [FromBody] PerformActionDto dto) =>
            AsActionResult(await _bookingService.CheckOutAsync(reservationId, dto.EmployeeId)); 
    }
}
