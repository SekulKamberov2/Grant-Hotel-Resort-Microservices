namespace GHR.Rating.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    using GHR.Rating.Application.Commands.AwardTopPerformers;
    using GHR.Rating.Application.Commands.CreateAward;
    using GHR.Rating.Application.Commands.DeleteAward;
    using GHR.Rating.Application.Commands.UpdateAward;
    using GHR.Rating.Application.Queries.GetAwardById;
    using GHR.Rating.Application.Queries.GetAwardsByPeriod;

    public class AwardController : BaseApiController
    {
        private readonly IMediator _mediator;
        public AwardController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAwardCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAwardCommand command)
        {
            if (id != command.Id) return BadRequest("Id mismatch.");
            return AsActionResult(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            AsActionResult(await _mediator.Send(new DeleteAwardCommand(id)));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) =>
            AsActionResult(await _mediator.Send(new GetAwardByIdQuery(id)));

        [HttpGet("period/{period}")]
        public async Task<IActionResult> GetByPeriod(string period) =>
            AsActionResult(await _mediator.Send(new GetAwardsByPeriodQuery(period)));

        // POST: generates automatic awards for the period (Weekly/Monthly/Yearly)
        [HttpPost("generate/{period}")]
        public async Task<IActionResult> GenerateAwards(string period) =>
            AsActionResult(await _mediator.Send(new AwardTopPerformersCommand(period)));
    }
}
