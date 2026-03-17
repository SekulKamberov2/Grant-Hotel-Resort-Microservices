namespace IdentityServer.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Dapper;

    using IdentityServer.Application.Exceptions;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Domain.Exceptions;
    using IdentityServer.Domain.Models;

    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(IConfiguration configuration, ILogger<RoleRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<bool> AddUserToRoleAsync(int userId, int roleId)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string deleteSql = "DELETE FROM UserRoles WHERE UserId = @UserId";
                await connection.ExecuteAsync(deleteSql, new { UserId = userId }, transaction);

                const string insertSql = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                var rowsInserted = await connection.ExecuteAsync(insertSql, new { UserId = userId, RoleId = roleId }, transaction);

                transaction.Commit();
                return rowsInserted > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error occurred while adding user ID {UserId} to role ID {RoleId}.", userId, roleId);
                throw new RepositoryException("Error occurred while updating user role.", ex);
            }
        }

        public async Task<bool> CreateUserRoleAsync(string roleName, string description)
        {
            using var connection = CreateConnection();
            const string sql = @"INSERT INTO Roles (Name, Description) VALUES (@Name, @Description);";
            var parameters = new { Name = roleName, Description = description };
            try
            {
                var result = await connection.ExecuteAsync(sql, parameters);
                if (result <= 0)
                    throw new RepositoryException("Failed to create a role.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role {RoleName}.", roleName);
                throw new RepositoryException("Error occurred while creating the role.", ex);
            }
        }

        public async Task<bool> DeleteUserRoleAsync(int roleId)
        {
            using var connection = CreateConnection();
            const string sql = @"DELETE FROM Roles WHERE Id = @RoleId";
            var parameters = new { RoleId = roleId };
            try
            {
                var result = await connection.ExecuteAsync(sql, parameters);
                if (result <= 0)
                    throw new RepositoryException("Role not found or already deleted.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting role ID {RoleId}.", roleId);
                throw new RepositoryException("Error occurred while deleting role.", ex);
            }
        }

        public async Task<Role> FindRoleByIdAsync(int roleId)
        {
            using var connection = CreateConnection();
            const string query = "SELECT * FROM Roles WHERE Id = @Id";
            var parameters = new { Id = roleId };
            try
            {
                var role = await connection.QuerySingleOrDefaultAsync<Role>(query, parameters);
                if (role == null)
                    throw new NotFoundException($"Role with ID {roleId} not found.");

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching role by ID {RoleId}.", roleId);
                throw new RepositoryException("Error occurred while retrieving role.", ex);
            }
        }

        public async Task<string> GetRoleForUserAsync(int userId)
        {
            using var connection = CreateConnection();
            const string query = "SELECT Role FROM Users WHERE Id = @UserId";
            var parameters = new { UserId = userId };
            try
            {
                return await connection.QueryFirstOrDefaultAsync<string>(query, parameters) ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching role for user ID {UserId}.", userId);
                throw new RepositoryException("Error occurred while retrieving user role.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            using var connection = CreateConnection();
            const string query = @"
                SELECT r.Name 
                FROM Roles r 
                LEFT JOIN UserRoles ur ON r.Id = ur.RoleId 
                WHERE ur.UserId = @UserId";
            var parameters = new { UserId = userId };
            try
            {
                var roles = await connection.QueryAsync<string>(query, parameters);
                if (roles == null || !roles.Any())
                    throw new RepositoryException("No roles found for the given user.");
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching roles for user ID {UserId}.", userId);
                throw new RepositoryException("Error occurred while fetching roles.", ex);
            }
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            using var connection = CreateConnection();
            const string query = @"SELECT * FROM Roles";
            try
            {
                var roles = await connection.QueryAsync<Role>(query);
                if (roles == null || !roles.Any())
                    throw new RepositoryException("No roles found.");
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching roles.");
                throw new RepositoryException("Error occurred while fetching roles.", ex);
            }
        }

        public async Task<bool> UpdateUserRoleAsync(int id, string? roleName, string? description)
        {
            using var connection = CreateConnection();
            var updates = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                updates.Add("Name = @Name");
                parameters.Add("Name", roleName);
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                updates.Add("Description = @Description");
                parameters.Add("Description", description);
            }
            if (updates.Count == 0)
                throw new RepositoryException("No Name and Description provided for update.");

            var sql = $"UPDATE Roles SET {string.Join(", ", updates)} WHERE Id = @Id;";

            try
            {
                var result = await connection.ExecuteAsync(sql, parameters);
                if (result <= 0)
                    throw new RepositoryException("Failed to update the role.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role ID {RoleId}.", id);
                throw new RepositoryException("Error occurred while updating the role.", ex);
            }
        }
    }
}
