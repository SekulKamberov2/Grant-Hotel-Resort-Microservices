namespace IdentityServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MediatR;

    using IdentityServer.Application.Commands.CreateUser;
    using IdentityServer.Application.Commands.DeleteUser;
    using IdentityServer.Application.Commands.UpdateUser;
    using IdentityServer.Application.Commands.CreateRole;
    using IdentityServer.Application.Commands.UpdateRole;
    using IdentityServer.Application.Commands.DeleteRole;
    using IdentityServer.Application.Commands.ResetUserPassword;
    using IdentityServer.Application.Commands.AssignRole;
    using IdentityServer.Application.Commands.SignIn;
    using IdentityServer.Application.Queries.GetUserInfo;
    using IdentityServer.Application.Queries.GetAllUsers;
    using IdentityServer.Application.Queries.GetAllRoles;

    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpPost("signup")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpPatch("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            return AsActionResult(await _mediator.Send(command));
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId) =>
            AsActionResult(await _mediator.Send(new DeleteUserCommand { UserId = userId }));

        [HttpGet("me/info/{userId}")]
        public async Task<IActionResult> GetInfo(int userId) =>
            AsActionResult(await _mediator.Send(new GetUserInfoQuery { UserId = userId }));

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsersAsync() =>
            AsActionResult<IEnumerable<GetQueryResponse>>(await _mediator.Send(new GetAllUsersQuery()));

        [HttpGet("admin/all-roles")]
        public async Task<IActionResult> GetAllRolesAsync() =>
            AsActionResult(await _mediator.Send(new GetAllRolesQuery()));

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetUserPasswordCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpPost("admin/reset-password")]
        public async Task<IActionResult> ResetPasswordByAdminAsync([FromBody] ResetUserPasswordCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleCommand command) =>
            AsActionResult(await _mediator.Send(command));

        [HttpPatch("update-role/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;
            return AsActionResult(await _mediator.Send(command));
        }

        [HttpDelete("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(int id) =>
            AsActionResult(await _mediator.Send(new DeleteRoleCommand { RoleId = id }));
    }
}
