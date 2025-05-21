namespace IdentityServer.Application.Commands.ResetUserPassword
{
    using MediatR; 
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;  
    public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, IdentityResult<bool>>
    {
        private readonly IUserManager _userManager;
        public ResetUserPasswordCommandHandler(IUserManager userManager) => _userManager = userManager;
        public async Task<IdentityResult<bool>> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (request == null) 
                return IdentityResult<bool>.Failure("Request cannot be null.");  
            try
            {  
                var userResult = await _userManager.FindByIdAsync(request.Id);
                if (!userResult.IsSuccess)
                    return IdentityResult<bool>.Failure("User not found."); 
                return await _userManager.ResetPasswordAsync(userResult.Data.Id, request.NewPassword);
            }
            catch (Exception ex)
            { 
                var errorMessage = $"Unexpected error occurred while resetting the password. Details: {ex.Message}";
                return IdentityResult<bool>.Failure(errorMessage);
            }
        }

    }
}
