namespace IdentityServer.Application.Commands.SignIn
{ 
    public record SignInResponse(string Token, AuthenticatedUser User); 
}
