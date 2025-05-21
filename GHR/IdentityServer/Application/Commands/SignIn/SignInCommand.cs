namespace IdentityServer.Application.Commands.SignIn
{ 
    using MediatR; 
    using IdentityServer.Application.Results; 
    public class SignInCommand : IRequest<IdentityResult<SignInResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
