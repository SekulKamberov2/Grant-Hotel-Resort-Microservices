namespace IdentityServer.Infrastructure.Identity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.Exceptions;
    using IdentityServer.Domain.Models; 

    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserManager> _logger;

        public UserManager(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger<UserManager> logger
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        private async Task<IdentityResult<T>> ExecuteWithLogging<T>(
            Func<Task<T>> action,
            string successMessage,
            string errorMessage,
            params object[] args)
        {
            try
            {
                var result = await action();
                if (EqualityComparer<T>.Default.Equals(result, default))
                {
                    // Return failure when result is null or default
                    return IdentityResult<T>.Failure(errorMessage);
                }
                _logger.LogInformation(successMessage, args);
                return IdentityResult<T>.Success(result);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, errorMessage, args);
                return IdentityResult<T>.Failure(errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while executing action.");
                throw; // Rethrow unexpected exceptions
            }
        }

        // Modify methods to handle nullable return types more effectively

        public async Task<IdentityResult<User?>> FindByIdAsync(int userId)
        {
            return await ExecuteWithLogging(
                () => _userRepository.GetUserByIdAsync(userId),
                "User with ID {UserId} found successfully.",
                "Error occurred while finding the user with ID {UserId}",
                userId
            );
        }

        public async Task<IdentityResult<IEnumerable<User>>> GetAllUsersAsync()
        {
            return await ExecuteWithLogging(
                () => _userRepository.GetUsersAsync(),
                "Fetched all users successfully.",
                "Error occurred while fetching users."
            );
        }

        public async Task<IdentityResult<User?>> CreateAsync(User user)
        {
            return await ExecuteWithLogging(
                async () =>
                {
                    var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
                    if (existingUser != null)
                    {
                        // User already exists, return null (failure case)
                        return null;
                    }

                    var hashedPassword = _passwordHasher.HashPassword(user.Password);
                    return await _userRepository.CreateUserAsync(user, hashedPassword);
                },
                "User {Email} created successfully.",
                "An error occurred while creating the user with email {Email}.",
                user.Email
            );
        }

        public async Task<IdentityResult<bool>> DeleteAsync(int userId)
        {
            return await ExecuteWithLogging(
                () => _userRepository.DeleteUserAsync(userId),
                "User with ID {UserId} deleted successfully.",
                "Failed to delete the user with ID {UserId}",
                userId
            );
        }

        public async Task<IdentityResult<User>> UpdateAsync(User user)
        {
            return await ExecuteWithLogging(
                () => _userRepository.UpdateUserAsync(user),
                "User with ID {UserId} updated successfully.",
                "Failed to update user with ID {UserId}",
                user.Id
            );
        }

        public async Task<IdentityResult<User?>> ValidateUserAsync(string email, string password)
        {
            return await ExecuteWithLogging(
                async () =>
                {
                    var user = await _userRepository.GetUserByEmailAsync(email);
                    if (user == null) return null;

                    var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, password);
                    return isPasswordValid ? user : null;
                },
                "User {Email} validated successfully.",
                "Invalid credentials for user with email {Email}.",
                email
            );
        }

        public async Task<IdentityResult<bool>> ResetPasswordAsync(int id, string newPassword)
        {
            return await ExecuteWithLogging(
                async () =>
                {
                    var hashedPassword = _passwordHasher.HashPassword(newPassword);
                    return await _userRepository.ResetPasswordAsync(id, hashedPassword);
                },
                "Password for user with ID {UserId} reset successfully.",
                "Error occurred while resetting the password for user ID {UserId}.",
                id
            );
        }

        public async Task<IdentityResult<User?>> FindByEmailAsync(string email)
        {
            return await ExecuteWithLogging(
                () => _userRepository.GetUserByEmailAsync(email),
                "Email {Email} check completed successfully.",
                "Error occurred while checking email {Email}.",
                email
            );
        }
    }
}
