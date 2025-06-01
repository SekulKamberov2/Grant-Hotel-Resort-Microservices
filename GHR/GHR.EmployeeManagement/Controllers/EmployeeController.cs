namespace GHR.EmployeeManagement.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    using GHR.EmployeeManagement.Application.Queries.GetAllEmployees;

    public class EmployeeController : BaseApiController
    {
        private readonly IMediator _mediator;
        public EmployeeController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            AsActionResult(await _mediator.Send(new GetAllEmployeesQuery()));








    }
}
