namespace IdentityServer.Application.Commands.SignIn
{
    public record AuthenticatedUser(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated, IEnumerable<string> Roles);
}
