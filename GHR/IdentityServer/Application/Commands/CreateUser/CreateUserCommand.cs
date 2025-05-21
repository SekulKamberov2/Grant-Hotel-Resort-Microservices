namespace IdentityServer.Application.Commands.CreateUser
{
    using MediatR; 
    using IdentityServer.Application.Results; 
    public record CreateUserCommand(string UserName, string Email, string Password, string PhoneNumber) : IRequest<IdentityResult<CreateUserResponse>>;
}
