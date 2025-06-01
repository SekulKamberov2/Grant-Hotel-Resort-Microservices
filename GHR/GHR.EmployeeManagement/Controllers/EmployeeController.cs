namespace GHR.EmployeeManagement.Controllers
{ 
    using Microsoft.AspNetCore.Mvc;

    using MediatR;

    using GHR.EmployeeManagement.Application.Commands.Create;
    using GHR.EmployeeManagement.Application.DTOs;
    using GHR.EmployeeManagement.Application.Queries.GetAllEmployees;
    using GHR.EmployeeManagement.Application.Queries.GetEmployeeById;
 

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




    }
}
