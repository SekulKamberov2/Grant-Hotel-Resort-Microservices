namespace IdentityServer.Application.Commands.UpdateUser
{
    using MediatR;
    using IdentityServer.Application.Results;  
    public class UpdateUserCommand : IRequest<IdentityResult<UpdateUserResponse>>
    {
        public int Id { get; set; } 
        public string? Email { get; set; }  
        public string? PhoneNumber { get; set; }
    } 
}
