namespace GHR.Rating.API.Controllers
{
    using MediatR;

    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    using GHR.Rating.Application.Commands;
    using GHR.Rating.Application.Commands.ApproveRating;
    using GHR.Rating.Application.Commands.BulkDeleteRatings;
    using GHR.Rating.Application.Commands.CreateRating;
    using GHR.Rating.Application.Commands.DeleteRating;
    using GHR.Rating.Application.Commands.FlagRating;
    using GHR.Rating.Application.Commands.RestoreRating;
    using GHR.Rating.Application.Commands.UpdateRating;
    using GHR.Rating.Application.Queries;
    using GHR.Rating.Application.Queries.GetAllRatings;
    using GHR.Rating.Application.Queries.GetRankingByPeriod;
    using GHR.Rating.Application.Queries.GetRatingById;
    using GHR.Rating.Application.Queries.GetRatingsByStatus;

    public class RatingController : BaseApiController
    {
        private readonly IMediator _mediator;
        public RatingController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Rate([FromBody] CreateRatingCommand command) =>
            AsActionResult(await _mediator.Send(command));
          
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) =>
            AsActionResult(await _mediator.Send(new GetRatingByIdQuery(id)));

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            AsActionResult(await _mediator.Send(new GetAllRatingsQuery()));
         
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId) =>
            AsActionResult(await _mediator.Send(new GetRatingsByUserQuery(userId)));

        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetByDepartment(int departmentId) =>
        AsActionResult(await _mediator.Send(new GetRatingsByDepartmentQuery(departmentId)));

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetByService(int serviceId) =>
            AsActionResult(await _mediator.Send(new GetRatingsByServiceQuery(serviceId)));

        [HttpGet("average/{departmentId}")]
        public async Task<IActionResult> GetAverage(int departmentId) =>
             AsActionResult(await _mediator.Send(new GetAverageRatingQuery(departmentId)));

        [HttpGet("ranking/{period}")] // period: weekly, monthly, yearly
        public async Task<IActionResult> GetRanking(string period) =>
            AsActionResult(await _mediator.Send(new GetRankingByPeriodQuery(period)));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            AsActionResult(await _mediator.Send(new DeleteRatingCommand(id)));

        //PUT /api/rating/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRatingCommand command)
        {
            if (id != command.Id) return BadRequest("ID in route does not match payload.");
            return AsActionResult(await _mediator.Send(command));
        }

        //POST /api/rating/{id}/approve
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id) =>
            AsActionResult(await _mediator.Send(new ApproveRatingCommand(id)));

        //POST /api/rating/{id}/flag
        [HttpPost("{id}/flag")]
        public async Task<IActionResult> Flag(int id, [FromBody] FlagRatingCommand command)
        {
            if (id != command.Id) return BadRequest();
            return AsActionResult(await _mediator.Send(command));
        }

        //POST /api/rating/bulk-delete Body: { "ratingIds": [1, 2, 3, 4] }
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRatingsCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpGet("status")]
        public async Task<IActionResult> GetByStatus([FromQuery] bool? isApproved = null, [FromQuery] bool? isFlagged = null, [FromQuery] bool? isDeleted = null) =>
             AsActionResult(await _mediator.Send(new GetRatingsByStatusQuery(isApproved, isFlagged, isDeleted)));

        // POST /api/rating/{id}/restore
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id) =>
            AsActionResult(await _mediator.Send(new RestoreRatingCommand(id)));

        // POST /api/rating/{id}/unflag
        [HttpPost("{id}/unflag")]
        public async Task<IActionResult> Unflag(int id) =>
            AsActionResult(await _mediator.Send(new UnflagRatingCommand(id)));
    }
}