namespace IdentityServer.Application.Commands.CreateRole
{
    using MediatR; 
    using IdentityServer.Application.Results; 
    public record CreateRoleCommand(string Name, string Description) : IRequest<IdentityResult<bool>>;

}
