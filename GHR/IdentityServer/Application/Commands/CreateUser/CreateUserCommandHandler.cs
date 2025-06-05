namespace IdentityServer.Application.Commands.CreateUser
{
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.DTOs;
    using IdentityServer.Domain.Models;
    using MediatR;
    using System.Text.Json;
    using System.Threading;

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

            var user = new UserDTO
            {
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role
            };
           
            var existingEmail = await _userManager.FindByEmailAsync(request.Email); 
            if (existingEmail.IsSuccess && existingEmail.Data != null) 
                return IdentityResult<CreateUserResponse>.Failure("Email already in use.");
           
            var createdUser = await _userManager.CreateAsync(user);
            if (!createdUser.IsSuccess)
                return IdentityResult<CreateUserResponse>.Failure("Failed to create user.");
         
            var roleId = request.Role != 0 ? request.Role : 1;
            var addRoleResult = await _roleManager.AddToRoleAsync(createdUser.Data.Id, roleId);  
            if (!addRoleResult.IsSuccess)
                return IdentityResult<CreateUserResponse>.Failure("Failed to assign role to user.");
             
            var response = new CreateUserResponse(
                createdUser.Data.Id,
                createdUser.Data.UserName,
                createdUser.Data.Email,
                createdUser.Data.PhoneNumber,
                createdUser.Data.DateCreated,
                request.Role
            );

            return IdentityResult<CreateUserResponse>.Success(response);
        }
    }

}
