namespace IdentityServer.Application.Commands.DeleteRole
{
    using MediatR; 
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;  
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IdentityResult<bool>>
    {
        private readonly IRoleManager _roleManager;
        public DeleteRoleCommandHandler(IRoleManager roleManager) => _roleManager = roleManager; 
        public async Task<IdentityResult<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _roleManager.GetRoleByIdAsync(request.RoleId);
            if (!result.IsSuccess) return IdentityResult<bool>.Failure("Role not found.");

            try
            {
                return await _roleManager.DeleteRoleAsync(result.Data.Id);
            }
            catch (Exception)
            {
                return IdentityResult<bool>.Failure("An unexpected error occurred while deleting the role.");
            }
        }
    }
}
