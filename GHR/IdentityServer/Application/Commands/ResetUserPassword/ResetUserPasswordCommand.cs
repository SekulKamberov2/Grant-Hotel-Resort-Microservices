namespace IdentityServer.Application.Commands.ResetUserPassword
{
    using MediatR;
    using IdentityServer.Application.Results; 
    public class ResetUserPasswordCommand : IRequest<IdentityResult<bool>>
    {
        public int Id { get; set; } 
        public string NewPassword { get; set; }
    }
}
