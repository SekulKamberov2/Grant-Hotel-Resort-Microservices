namespace IdentityServer.Application.Commands.SignIn
{
    
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;  
    using MediatR; 
    public class SignInCommandHandler : IRequestHandler<SignInCommand, IdentityResult<SignInResponse>> 
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;
        private readonly ITokenService _tokenService;

        public SignInCommandHandler(
            IUserManager userManager, 
            IRoleManager roleManager, 
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult<SignInResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
             
            var user = await _userManager.ValidateUserAsync(request.Email, request.Password);
            if (!user.IsSuccess)
                return IdentityResult<SignInResponse>.Failure("Invalid credentials");
             
            var roles = await _roleManager.GetRolesAsync(user.Data.Id);  
            var roleList = roles.Data?.Any() == true ? roles.Data : new[] { "EMPLOYEE" }; // hmmm
             
            var tokenResult = _tokenService.GenerateToken(user.Data.Id.ToString(), user.Data, roleList);
            if (!tokenResult.IsSuccess)
                return IdentityResult<SignInResponse>.Failure(tokenResult.Error ?? "Token generation failed");
             
            return IdentityResult<SignInResponse>.Success(
                new SignInResponse(tokenResult.Data, new AuthenticatedUser(
                    user.Data.Id, user.Data.UserName, user.Data.Email,
                    user.Data.PhoneNumber, user.Data.DateCreated, roleList)));
        }

    }
}
