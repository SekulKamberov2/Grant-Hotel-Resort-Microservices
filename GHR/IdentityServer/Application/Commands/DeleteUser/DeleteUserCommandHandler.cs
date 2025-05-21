namespace IdentityServer.Application.Commands.DeleteUser
{
    using MediatR;

    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IdentityResult<bool>>
    {
        private readonly IUserManager _userManager; 
        public DeleteUserCommandHandler(IUserManager userManager) => _userManager = userManager;
         
        public async Task<IdentityResult<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.FindByIdAsync(request.UserId);
            if (!result.IsSuccess)
                return IdentityResult<bool>.Failure("User not found.");

            try
            { 
                return await _userManager.DeleteAsync(request.UserId);
            }
            catch (Exception)
            {
                return IdentityResult<bool>.Failure("An unexpected error occurred while deleting the user.");
            }
        }
    } 
}
