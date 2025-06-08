namespace GHR.DFM.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using GHR.DFM.DTOs;
    using GHR.DFM.Entities;
    using GHR.DFM.Services;

    public class FacilitiesController : BaseApiController
    {
        private readonly IFacilityService _service; 
        public FacilitiesController(IFacilityService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            AsActionResult(await _service.GetAllAsync()); 

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) =>
            AsActionResult(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Facility facility) =>
            AsActionResult(await _service.CreateAsync(facility));  

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Facility facility) =>
            AsActionResult(await _service.UpdateAsync(facility));  

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            AsActionResult(await _service.DeleteAsync(id));   

        [HttpGet("types")]
        public async Task<IActionResult> GetFacilityTypes() =>
            AsActionResult(await _service.GetFacilityTypesAsync()); 

        //facilities/status
        [HttpGet("status")]
        public async Task<IActionResult> GetFacilityStatuses() =>
            AsActionResult(await _service.GetFacilityStatusesAsync());

        //facilities/{id}/status body-string only:  "Under Maintenance"
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateFacilityStatus(int id, [FromBody] string status) =>
            AsActionResult(await _service.UpdateFacilityStatusAsync(id, status));  
         
        //facilities/available
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableFacilities() =>
            AsActionResult(await _service.GetAvailableFacilitiesAsync());

        //facilities/schedule
        [HttpPost("schedule")]
        public async Task<IActionResult> CreateFacilityScheduleAsync([FromBody] FacilitySchedule schedules) =>
            AsActionResult(await _service.CreateFacilityScheduleAsync(schedules));

        //facilities/{id}/schedule
        [HttpGet("{id}/schedule")]
        public async Task<IActionResult> GetFacilitySchedule(int id) =>
            AsActionResult(await _service.GetFacilityScheduleAsync(id));  

        //facilities/{id}/schedule
        [HttpPut("{id}/schedule")]
        public async Task<IActionResult> UpdateFacilitySchedule(int id, [FromBody] IEnumerable<FacilitySchedule> schedules) =>
            AsActionResult(await _service.UpdateFacilityScheduleAsync(id, schedules)); 
          
        //facilities/nearby?location=Downtown
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyFacilities([FromQuery] string location) =>
            AsActionResult(await _service.GetNearbyFacilitiesAsync(location)); 

        //facilities/{id}/services
        [HttpGet("{id}/services")]
        public async Task<IActionResult> GetFacilityServices(int id) =>
            AsActionResult(await _service.GetFacilityServicesAsync(id));  

        //facilities/{id}/services
        [HttpPost("{id}/services")]
        public async Task<IActionResult> AddFacilityService(int id, [FromBody] FacilityServiceItem service) =>
            AsActionResult(await _service.AddFacilityServiceAsync(service));  

        //facilities/{id}/services/{sid}
        [HttpDelete("{id}/services/{sid}")]
        public async Task<IActionResult> DeleteService(int id, int sid) =>
            AsActionResult(await _service.DeleteFacilityServiceAsync(id, sid));   

        //facilities/{id}/reserve
        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveFacility(int id, [FromBody] FacilityReservation reservation) =>
            AsActionResult(await _service.CreateReservationAsync(reservation));
        

        //facilities/{id}/reservations
        [HttpGet("{id}/reservations")]
        public async Task<IActionResult> GetReservations(int id) =>
            AsActionResult(await _service.GetReservationsByFacilityAsync(id));
         
        //facilities/{id}/reservations/{resId}
        [HttpDelete("{id}/reservations/{resId}")]
        public async Task<IActionResult> CancelReservation(int id, int resId) =>
            AsActionResult(await _service.DeleteReservationAsync(id, resId)); 

        //facilities/{id}/report-issue
        [HttpPost("{id}/report-issue")]
        public async Task<IActionResult> ReportIssue(int id, [FromBody] FacilityIssue issue) =>
            AsActionResult(await _service.ReportIssueAsync(issue));  
         
        //facilities/{id}/issues
        [HttpGet("{id}/issues")]
        public async Task<IActionResult> GetIssues(int id) =>
            AsActionResult(await _service.GetOpenIssuesAsync(id));  

        [HttpPut("{id}/assign-maintenance")]
        public async Task<IActionResult> AssignMaintenance(int id, [FromBody] AssignMaintenanceDto dto) =>
            AsActionResult(await _service.AssignMaintenanceAsync(id, dto.IssueId, dto.AssignedTo));   

        [HttpGet("{id}/usage-history")]
        public async Task<IActionResult> GetUsageHistory(int id) =>
            AsActionResult(await _service.GetUsageHistoryAsync(id));    

        [HttpGet("{id}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(int id, [FromQuery] DateTime date) =>
            AsActionResult(await _service.GetAvailableSlotsAsync(id, date)); 
    }
}
