namespace GHR.DutyManagement.API.Controllers
{ 
    using Microsoft.AspNetCore.Mvc;
     
    using MediatR;

    using GHR.DutyManagement.Application.Commands.AssignDuty;

    public class DutiesController : BaseApiController
    {
        private readonly IMediator _mediator;
        public DutiesController(IMediator mediator) => _mediator = mediator;

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] AssignDutyCommand command) =>
            AsActionResult(await _mediator.Send(command));


        //[HttpPost("assign")]
        //public async Task<IActionResult> AssignTask([FromBody] AssignDutyCommand command) =>
        //    AsActionResult(await _mediator.Send(command));

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var result = await _mediator.Send(new GetAllDutiesQuery());
        //    return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode ?? 500, result.Error);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var result = await _mediator.Send(new GetDutyByIdQuery(id));
        //    return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode ?? 500, result.Error);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateDutyCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data)
        //                            : StatusCode(result.StatusCode ?? 500, result.Error);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] UpdateDutyCommand command)
        //{
        //    if (id != command.Id)
        //        return BadRequest("ID mismatch between route and body");

        //    var result = await _mediator.Send(command);
        //    return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode ?? 500, result.Error);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _mediator.Send(new DeleteDutyCommand(id));
        //    return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode ?? 500, result.Error);
        //}



        //[HttpPost("{id}/complete")]
        //public async Task<IActionResult> MarkAsComplete(int id)
        //{
        //    var result = await _mediator.Send(new CompleteDutyCommand(id));
        //    return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode ?? 500, result.Error);
        //}
    }
}
