namespace IdentityServer.Infrastructure.Identity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.Exceptions;
    using IdentityServer.Domain.Models;
    using Microsoft.Extensions.Logging;

    public class RoleManager : IRoleManager
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleManager> _logger;

        public RoleManager(IRoleRepository roleRepository, ILogger<RoleManager> logger)
        {
            _roleRepository = roleRepository;
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

        public async Task<IdentityResult<IEnumerable<Role>>> GetAllRolesAsync()
        {
            return await ExecuteWithLogging(
                () => _roleRepository.GetRolesAsync(),
                "Fetched all roles successfully.",
                "Error occurred while fetching roles."
            );
        }

        public async Task<IdentityResult<IEnumerable<string>>> GetRolesAsync(int userId)
        {
            return await ExecuteWithLogging(
                () => _roleRepository.GetUserRolesAsync(userId),
                "Fetched user roles successfully.",
                $"Error occurred while fetching roles for user ID {userId}."
            );
        }

        public async Task<IdentityResult<bool>> AddToRoleAsync(int userId, int roleId)
        {
            return await ExecuteWithLogging(
                () => _roleRepository.AddUserToRoleAsync(userId, roleId),
                "User added to role successfully.",
                $"Error occurred while adding user ID {userId} to role ID {roleId}."
            );
        }

        public async Task<IdentityResult<bool>> CreateRoleAsync(string name, string description)
        {
            return await ExecuteWithLogging(
                () => _roleRepository.CreateUserRoleAsync(name, description),
                "Role created successfully.",
                $"Error occurred while creating role {name}."
            );
        }

        public async Task<IdentityResult<bool>> UpdateRoleAsync(int id, string? name, string? description)
        {
            return await ExecuteWithLogging(
                async () =>
                {
                    var role = await _roleRepository.FindRoleByIdAsync(id);
                    if (role == null)
                        return false;

                    return await _roleRepository.UpdateUserRoleAsync(id, name, description);
                },
                "Role updated successfully.",
                $"Error occurred while updating role ID {id}."
            );
        }

        public async Task<IdentityResult<bool>> DeleteRoleAsync(int roleId)
        {
            return await ExecuteWithLogging(
                () => _roleRepository.DeleteUserRoleAsync(roleId),
                "Role deleted successfully.",
                $"Error occurred while deleting role ID {roleId}."
            );
        }

        public async Task<IdentityResult<Role>> GetRoleByIdAsync(int roleId)
        {
            return await ExecuteWithLogging(
                () => _roleRepository.FindRoleByIdAsync(roleId),
                "Fetched role by ID successfully.",
                $"Error occurred while fetching role ID {roleId}."
            );
        }
    }
}
