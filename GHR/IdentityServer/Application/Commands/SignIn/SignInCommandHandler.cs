namespace IdentityServer.Application.Commands.SignIn
{
    using MediatR; 
    
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;  
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

            // Validate user credentials
            var user = await _userManager.ValidateUserAsync(request.Email, request.Password);
            if (!user.IsSuccess)
                return IdentityResult<SignInResponse>.Failure("Invalid credentials");

            // Retrieve user roles
            var roles = await _roleManager.GetRolesAsync(user.Data.Id);  
            var roleList = roles.Data ?? Enumerable.Empty<string>();

            // Generate token for user
            var tokenResult = _tokenService.GenerateToken(user.Data.Id.ToString(), user.Data, roleList);
            if (!tokenResult.IsSuccess)
                return IdentityResult<SignInResponse>.Failure(tokenResult.Error ?? "Token generation failed");

            // Return successful response with token and user information
            return IdentityResult<SignInResponse>.Success(
                new SignInResponse(tokenResult.Data, new AuthenticatedUser(
                    user.Data.Id, user.Data.UserName, user.Data.Email,
                    user.Data.PhoneNumber, user.Data.DateCreated, roleList)));
        }

    }
}
