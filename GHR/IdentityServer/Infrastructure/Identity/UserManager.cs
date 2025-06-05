namespace IdentityServer.Infrastructure.Identity
{
    using AutoMapper;
    using Google.Protobuf.WellKnownTypes;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.DTOs;
    using IdentityServer.Domain.Exceptions;
    using IdentityServer.Domain.Models;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserManager> _logger;
        private readonly IMapper _mapper;

        public UserManager(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger<UserManager> logger,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _mapper = mapper;
        }

        //private async Task<IdentityResult<T>> ExecuteWithLogging<T>(
        //    Func<Task<T>> action,
        //    string successMessage,
        //    string errorMessage,
        //    params object[] args)
        //{
        //    try
        //    {
        //        var result = await action();
        //        if (EqualityComparer<T>.Default.Equals(result, default))
        //        {
        //            // Return failure when result is null or default
        //            return IdentityResult<T>.Failure(errorMessage);
        //        }
        //        _logger.LogInformation(successMessage, args);
        //        return IdentityResult<T>.Success(result);
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        _logger.LogError(ex, errorMessage, args);
        //        return IdentityResult<T>.Failure(errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error occurred while executing action.");
        //        throw; // Rethrow unexpected exceptions
        //    }
        //}

        private async Task<IdentityResult<TDestination>> ExecuteWithLogging<TSource, TDestination>(
            Func<Task<TSource>> action,
            string successMessage,
            string errorMessage,
            IMapper mapper,
            params object[] args)
        {
            try
            {
                var result = await action();

                //checks whether the result returned from your action() is the default value of TSource
                if (EqualityComparer<TSource>.Default.Equals(result, default)) 
                    return IdentityResult<TDestination>.Failure(errorMessage); 

                var mappedResult = mapper.Map<TDestination>(result);

                _logger.LogInformation(successMessage, args);
                return IdentityResult<TDestination>.Success(mappedResult);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, errorMessage, args);
                return IdentityResult<TDestination>.Failure(errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while executing action.");
                throw;
            }
        }


        // Modify methods to handle nullable return types more effectively

        public async Task<IdentityResult<UserDTO>> FindByIdAsync(int userId)
        {
            return await ExecuteWithLogging<User, UserDTO>(
                () => _userRepository.GetUserByIdAsync(userId),
                "User with ID {UserId} found successfully.",
                "Error occurred while finding the user with ID {UserId}",
                 _mapper,  
                userId
            );
        }

        //public async Task<IdentityResult<IEnumerable<UserDTO>>> GetAllUsersAsync()
        //{
        //    return await ExecuteWithLogging<User, UserDTO>(
        //        () => _userRepository.GetUsersAsync(),
        //        "Fetched all users successfully.",
        //        "Error occurred while fetching users."
        //    );
        //}
        public async Task<IdentityResult<IEnumerable<User>>> GetAllUsersAsync()
        {
            try
            { 
                return IdentityResult<IEnumerable<User>>.Success(await _userRepository.GetUsersAsync());
            }
            catch (Exception ex)
            { 
                return IdentityResult<IEnumerable<User>>.Failure("Error occurred while fetching users.");
            }
        }


        //public async Task<IdentityResult<UserDTO>> CreateAsync(User user)
        //{
        //    return await ExecuteWithLogging<User, UserDTO>(
        //        async () =>
        //        {
        //            var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
        //            if (existingUser != null) return null;  
        //            var hashedPassword = _passwordHasher.HashPassword(user.Password);
        //            return await _userRepository.CreateUserAsync(user, hashedPassword);
        //        },
        //        "User {Email} created successfully.",
        //        "An error occurred while creating the user with email {Email}.",
        //        _mapper,
        //        user.Email
        //    );
        //}

        public async Task<IdentityResult<UserDTO>> CreateAsync(UserDTO user)
        {
            return await ExecuteWithLogging<User, UserDTO>(
                async () =>
                {
                    var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
                    if (existingUser != null) return null; 

                    var hashedPassword = _passwordHasher.HashPassword(user.Password); 
                    var userEntity = _mapper.Map<User>(user); 
                    userEntity.PasswordHash = hashedPassword;

                    return await _userRepository.CreateUserAsync(userEntity, userEntity.PasswordHash);
                },
                "User {Email} created successfully.",
                "An error occurred while creating the user with email {Email}.",
                _mapper,
                user.Email
            );
        }


        public async Task<IdentityResult<bool>> DeleteAsync(int userId)
        {
            try
            {
                var result = await _userRepository.DeleteUserAsync(userId);

                if (result)
                {
                    _logger.LogInformation("User with ID {UserId} deleted successfully.", userId);
                    return IdentityResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Failed to delete the user with ID {UserId}.", userId);
                    return IdentityResult<bool>.Failure($"Failed to delete the user with ID {userId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the user with ID {UserId}.", userId);
                return IdentityResult<bool>.Failure($"Error occurred while deleting the user with ID {userId}.");
            }
        }


        public async Task<IdentityResult<UserDTO>> UpdateAsync(UserDTO user)
        {
            return await ExecuteWithLogging<User, UserDTO>(
                () => {
                    var userEntity = _mapper.Map<User>(user);  
                    var updatedUser = _userRepository.UpdateUserAsync(userEntity);
                    return updatedUser;
                },
                "User with ID {UserId} updated successfully.",
                "Failed to update user with ID {UserId}",
                _mapper,
                user.Id
            );
        }

        public async Task<IdentityResult<User>> ValidateUserAsync(string email, string password)
        {
            return await ExecuteWithLogging<User, User>(
                async () =>
                {
                    var user = await _userRepository.GetUserByEmailAsync(email);
                    if (user == null) return null;

                    var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, password);
                    return isPasswordValid ? user : null;
                },
                "User {Email} validated successfully.",
                "Invalid credentials for user with email {Email}.",
                _mapper,
                email
            );
        }

        //public async Task<IdentityResult<bool>> ResetPasswordAsync(int id, string newPassword)
        //{
        //    return await ExecuteWithLogging(
        //        async () =>
        //        {
        //            var hashedPassword = _passwordHasher.HashPassword(newPassword);
        //            return await _userRepository.ResetPasswordAsync(id, hashedPassword);
        //        },
        //        "Password for user with ID {UserId} reset successfully.",
        //        "Error occurred while resetting the password for user ID {UserId}.",
        //        id
        //    );
        //}
        public async Task<IdentityResult<bool>> ResetPasswordAsync(int id, string newPassword)
        {
            return await ExecuteWithLogging<bool, bool>(
                async () =>
                {
                    var hashedPassword = _passwordHasher.HashPassword(newPassword);
                    return await _userRepository.ResetPasswordAsync(id, hashedPassword);
                },
                "Password for user with ID {UserId} reset successfully.",
                "Error occurred while resetting the password for user ID {UserId}.",
                _mapper,
                id
            ); 
        }



        public async Task<IdentityResult<UserDTO>> FindByEmailAsync(string email)
        {
            return await ExecuteWithLogging<User, UserDTO>(
                () => _userRepository.GetUserByEmailAsync(email),
                "Email {Email} check completed successfully.",
                "Error occurred while checking email {Email}.",
                _mapper,
                email
            );
        }
    }
}
