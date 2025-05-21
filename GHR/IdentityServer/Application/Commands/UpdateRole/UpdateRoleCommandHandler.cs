namespace IdentityServer.Application.Commands.UpdateRole
{
    using MediatR; 
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.Exceptions;
    using IdentityServer.Infrastructure.Repositories;

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IdentityResult<bool>>
    {
        private readonly IRoleManager _roleManager;
        public UpdateRoleCommandHandler(IRoleManager roleManager) => _roleManager = roleManager;
        public async Task<IdentityResult<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested(); 
                var role = await _roleManager.GetRoleByIdAsync(request.Id);

                if (role == null) 
                    return IdentityResult<bool>.Failure("Failed to update role.");  
                 
                var updateResult = await _roleManager.UpdateRoleAsync(request.Id, request.Name, request.Description);

                if (!updateResult.IsSuccess) 
                    return IdentityResult<bool>.Failure("Failed to update role.");   

                return IdentityResult<bool>.Success(true);  
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure("Failed to update role.");  
            }
        } 
    }
}
