namespace IdentityServer.Application.Commands.CreateUser
{
    public record CreateUserResponse(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated);
} 
    
