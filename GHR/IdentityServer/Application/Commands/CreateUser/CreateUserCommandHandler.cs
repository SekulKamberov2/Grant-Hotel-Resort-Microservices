namespace IdentityServer.Application.Commands.CreateUser
{
    using System.Threading;

    using MediatR;

    using IdentityServer.Domain.Models;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IdentityResult<CreateUserResponse>>
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;

        public CreateUserCommandHandler(IUserManager userManager, IRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };
             
            var existingEmail = await _userManager.FindByEmailAsync(request.Email); 
            if (existingEmail.IsSuccess && existingEmail.Data != null) 
                return IdentityResult<CreateUserResponse>.Failure("Email already in use."); 
             
            // Create the user
            var createdUser = await _userManager.CreateAsync(user);
            if (!createdUser.IsSuccess)
                return IdentityResult<CreateUserResponse>.Failure("Failed to create user.");

            // Assign role after creating the user
            var addRoleResult = await _roleManager.AddToRoleAsync(createdUser.Data.Id, 3); // Assuming roleId 3 is valid
            if (!addRoleResult.IsSuccess)
                return IdentityResult<CreateUserResponse>.Failure("Failed to assign role to user.");

            // Map the created user to the response model
            var response = new CreateUserResponse(
                createdUser.Data.Id,
                createdUser.Data.UserName,
                createdUser.Data.Email,
                createdUser.Data.PhoneNumber,
                createdUser.Data.DateCreated
            );

            return IdentityResult<CreateUserResponse>.Success(response);
        }
    }

}
