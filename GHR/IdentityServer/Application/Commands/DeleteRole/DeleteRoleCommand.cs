namespace IdentityServer.Application.Commands.DeleteRole
{
    using MediatR;
    using IdentityServer.Application.Results;
    public class DeleteRoleCommand : IRequest<IdentityResult<bool>>
    {
        public int RoleId { get; set; }
    }
} 
