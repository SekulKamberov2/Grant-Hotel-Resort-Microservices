namespace GHR.DutyManagement.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using GHR.DutyManagement.DTOs;
    using GHR.DutyManagement.Entities;
    using GHR.DutyManagement.Services;
    public class DutiesController : BaseController
    {
        private readonly IDutyService _dutyservice; 
        public DutiesController(IDutyService dutyservice) => _dutyservice = dutyservice;

        [HttpGet]
        public async Task<IActionResult> GetAllDuties() =>
          AsActionResult(await _dutyservice.GetAllDutiesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDutyById(int id) =>
            AsActionResult(await _dutyservice.GetDutyByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> CreateDuty([FromBody] DutyDTO duty) =>
            AsActionResult(await _dutyservice.CreateDutyAsync(duty));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDuty(int id, [FromBody] Duty duty)
        {
            if (id != duty.Id) return BadRequest("Id mismatch");
            return AsActionResult(await _dutyservice.UpdateDutyAsync(duty));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDuty(int id) =>
            AsActionResult(await _dutyservice.DeleteDutyAsync(id));

        [HttpGet("shifts")]
        public async Task<IActionResult> GetAllShifts() =>
          AsActionResult(await _dutyservice.GetAllShiftsAsync());

        [HttpGet("period-types")]
        public async Task<IActionResult> GetAllPeriodTypes() =>
            AsActionResult(await _dutyservice.GetAllPeriodTypesAsync());

        [HttpGet("{dutyId}/assignments")]
        public async Task<IActionResult> GetDutyAssignments(int dutyId) =>
            AsActionResult(await _dutyservice.GetDutyAssignmentsAsync(dutyId));

        [HttpGet("available-staff/{facility}")]
        public async Task<IActionResult> GetAvailableStaff(string facility) =>
            AsActionResult(await _dutyservice.GetAvailableStaffAsync(facility));

        [HttpPost("assign")]
        public async Task<IActionResult> AssignDuty([FromBody] DutyAssignmentDTO dutyAssignment) =>
            AsActionResult(await _dutyservice.AssignDutyAsync(dutyAssignment));

        //
        [HttpGet("housekeeping/facility/{facility}/status/{status}")]
        public async Task<IActionResult> GetAllHouseKeeping(string facility, string status) =>
            AsActionResult(await _dutyservice.GetByFacilityAndStatusAsync(facility, status));
    }
}
