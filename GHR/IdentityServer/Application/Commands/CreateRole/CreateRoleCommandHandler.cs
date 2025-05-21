namespace IdentityServer.Application.Commands.CreateRole
{
    using MediatR;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results; 

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IdentityResult<bool>>
    {
        private readonly IRoleManager _roleManager; 
        public CreateRoleCommandHandler(IRoleManager roleManager) => _roleManager = roleManager; 
        public async Task<IdentityResult<bool>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _roleManager.CreateRoleAsync(request.Name, request.Description);
        }     
    } 
}
