namespace GHR.EmployeeManagement.Controllers
{
    using GHR.EmployeeManagement.Application.Commands.Create;
    using GHR.EmployeeManagement.Application.Commands.Delete; 
    using GHR.EmployeeManagement.Application.Commands.Update;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Queries.GetAllEmployees;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeeById;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesByDepartment;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesByFacility;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesByManager;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesHiredAfter;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesSalaryAbove;
    using GHR.EmployeeManagement.Application.Queries.Search;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
  
    public class EmployeeController : BaseApiController
    {
        private readonly IMediator _mediator;
        public EmployeeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            AsActionResult(await _mediator.Send(new GetAllEmployeesQuery()));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) =>
            AsActionResult(await _mediator.Send(new GetEmployeeByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDTO dto) =>
            AsActionResult(await _mediator.Send(new CreateEmployeeCommand(dto)));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDTO dto) =>
            AsActionResult(await _mediator.Send(new UpdateEmployeeCommand(id, dto)));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            AsActionResult(await _mediator.Send(new DeleteEmployeeCommand(id)));

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name) =>
            AsActionResult(await _mediator.Send(new SearchEmployeesByNameQuery(name)));

        [HttpGet("department/{departmentId:int}")]
        public async Task<IActionResult> GetByDepartment(int departmentId) =>
            AsActionResult(await _mediator.Send(new GetEmployeesByDepartmentQuery(departmentId)));

        [HttpGet("facility/{facilityId:int}")]
        public async Task<IActionResult> GetByFacility(int facilityId) =>
            AsActionResult(await _mediator.Send(new GetEmployeesByFacilityQuery(facilityId)));

        [HttpGet("hiredafter/{date}")]
        public async Task<IActionResult> GetHiredAfter(DateTime date) =>
            AsActionResult(await _mediator.Send(new GetEmployeesHiredAfterQuery(date)));

        [HttpGet("salaryabove/{salary:decimal}")]
        public async Task<IActionResult> GetSalaryAbove(decimal salary) =>
            AsActionResult(await _mediator.Send(new GetEmployeesSalaryAboveQuery(salary)));

        [HttpGet("manager/{managerId:int}")]
        public async Task<IActionResult> GetByManager(int managerId)
            =>
            AsActionResult(await _mediator.Send(new GetEmployeesByManagerQuery(managerId)));






    }
}
