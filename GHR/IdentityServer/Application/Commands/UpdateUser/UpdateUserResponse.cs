namespace IdentityServer.Application.Commands.UpdateUser
{
    public record UpdateUserResponse(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated);
}
