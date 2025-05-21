namespace IdentityServer.Application.Commands.UpdateUser
{
    using MediatR; 

    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results; 

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IdentityResult<UpdateUserResponse>>
    {
        private readonly IUserManager _userManager; 
        public UpdateUserCommandHandler(IUserManager userManager) => _userManager = userManager; 

        public async Task<IdentityResult<UpdateUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _userManager.FindByIdAsync(request.Id); 
            if (!result.IsSuccess) return IdentityResult<UpdateUserResponse>.Failure("User not found.");  

            var user = result.Data;   
            if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber; 

            var updatedUser = await _userManager.UpdateAsync(user);
            var responseResult = updatedUser.Map(user => new UpdateUserResponse(user.Id, user.UserName, user.Email, user.PhoneNumber, user.DateCreated));
            return responseResult;
        }
    } 
}
