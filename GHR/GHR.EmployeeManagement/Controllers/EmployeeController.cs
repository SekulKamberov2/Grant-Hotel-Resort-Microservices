namespace GHR.EmployeeManagement.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Grpc.Core;
    using MediatR;
 
    using GHR.EmployeeManagement.Application.Commands.Create;
    using GHR.EmployeeManagement.Application.Commands.Delete;
    using GHR.EmployeeManagement.Application.Commands.IncreaseSalary;
    using GHR.EmployeeManagement.Application.Commands.Update;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Queries.GetAllEmployees;
    using GHR.EmployeeManagement.Application.Queries.GetBirthdaysThisMonth;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeeById;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeeLeaveRequests;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesByDepartment;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesByFacility;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesByManager;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesByStatus;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesHiredAfter;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeesSalaryAbove;
    using GHR.EmployeeManagement.Application.Queries.Search;
   
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

        //http://localhost:7010/api/Employee/hiredafter/2021-12-22
        [HttpGet("hiredafter/{date}")]  //to alter HiredData
        public async Task<IActionResult> GetHiredAfter(DateTime date) =>
            AsActionResult(await _mediator.Send(new GetEmployeesHiredAfterQuery(date)));

        [HttpGet("salaryabove/{salary:decimal}")]
        public async Task<IActionResult> GetSalaryAbove(decimal salary) =>
            AsActionResult(await _mediator.Send(new GetEmployeesSalaryAboveQuery(salary)));

        [HttpGet("manager/{managerId:int}")]
        public async Task<IActionResult> GetByManager(int managerId) =>
            AsActionResult(await _mediator.Send(new GetEmployeesByManagerQuery(managerId)));


        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status) =>
            AsActionResult(await _mediator.Send(new GetEmployeesByStatusQuery(status)));

        [HttpGet("birthdays/month")]
        public async Task<IActionResult> GetBirthdaysThisMonth() =>
            AsActionResult(await _mediator.Send(new GetBirthdaysThisMonthQuery()));

        [HttpPut("increase-salary")]
        public async Task<IActionResult> IncreaseSalary([FromQuery] int years, [FromQuery] decimal percentage) =>
            AsActionResult(await _mediator.Send(new IncreaseSalaryCommand(years, percentage))); 
        
        [HttpGet("all-requests/{UserId:int}")]
        public async Task<IActionResult> GetEmployeeLeaveRequests(int userId) =>
            AsActionResult(await _mediator.Send(new GetEmployeeLeaveRequestsQuery(userId))); 
    }
}
