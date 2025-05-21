namespace IdentityServer.Application.Commands.DeleteUser
{
    using MediatR;

    using IdentityServer.Application.Results; 
    public class DeleteUserCommand : IRequest<IdentityResult<bool>>
    {
        public int UserId { get; set; }
    }
}
