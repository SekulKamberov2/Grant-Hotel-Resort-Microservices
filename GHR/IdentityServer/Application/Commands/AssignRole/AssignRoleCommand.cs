namespace IdentityServer.Application.Commands.AssignRole
{
    using MediatR;
    using IdentityServer.Application.Results; 
    public record AssignRoleCommand(int UserId, int RoleId) : IRequest<IdentityResult<bool>>;
}
