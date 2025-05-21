namespace IdentityServer.Application.Commands.UpdateRole
{ 
    using MediatR;
    using IdentityServer.Application.Results;  
    public class UpdateRoleCommand : IRequest<IdentityResult<bool>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }  
    }
}
