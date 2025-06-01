namespace GHR.EmployeeManagement.Controllers
{ 
    using GHR.EmployeeManagement.Application.Commands.Create;
    using GHR.EmployeeManagement.Application.Commands.Delete;
    using GHR.EmployeeManagement.Application.Commands.Update;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Queries.GetAllEmployees;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeeById;
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
    }
}
