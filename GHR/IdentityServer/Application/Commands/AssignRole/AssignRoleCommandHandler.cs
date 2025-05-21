namespace IdentityServer.Application.Commands.AssignRole
{
    using IdentityServer.Application.Commands.ResetUserPassword;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    using MediatR;

    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, IdentityResult<bool>>
    {
        private readonly IRoleManager _roleManager;
        private readonly IUserManager _userManager;
        public AssignRoleCommandHandler(IRoleManager roleManager, IUserManager userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IdentityResult<bool>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); 
            
            var userResult = await _userManager.FindByIdAsync(request.UserId);
            if (!userResult.IsSuccess) return IdentityResult<bool>.Failure("User not found.");
            
            return await _roleManager.AddToRoleAsync(userResult.Data.Id, request.RoleId); ;
        }
    }
}
