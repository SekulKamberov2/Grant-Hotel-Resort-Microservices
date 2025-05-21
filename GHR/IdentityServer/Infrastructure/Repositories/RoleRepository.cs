namespace IdentityServer.Infrastructure.Repositories
{
    using System.Data;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;

    using IdentityServer.Application.Interfaces;
    using IdentityServer.Domain.Models;
    using IdentityServer.Domain.Exceptions;

    using Dapper; 

    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(IConfiguration configuration, ILogger<RoleRepository> logger)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
        } 

        public async Task<bool> AddUserToRoleAsync(int userId, int roleId)
        {
            const string deleteQuery = "DELETE FROM UserRoles WHERE UserId = @UserId";
            const string insertQuery = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            var parameters = new { UserId = userId, RoleId = roleId };

            using var connection = _dbConnection;
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Delete previous roles
                await connection.ExecuteAsync(deleteQuery, new { UserId = userId }, transaction);

                // Insert the new role
                var rowsInserted = await connection.ExecuteAsync(insertQuery, parameters, transaction);

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
            const string sql = @"INSERT INTO Roles (Name, Description) VALUES (@Name, @Description);";
            var parameters = new { Name = roleName, Description = description };
            try
            {
                var result = await _dbConnection.ExecuteAsync(sql, parameters);
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
            const string sql = @"DELETE FROM Roles WHERE Id = @RoleId";
            var parameters = new { RoleId = roleId };
            try
            {
                var result = await _dbConnection.ExecuteAsync(sql, parameters);
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

        public async Task<Role?> FindRoleByIdAsync(int roleId)
        {
            const string query = "SELECT * FROM Roles WHERE Id = @Id";
            var parameters = new { Id = roleId };
            try
            {
                return await _dbConnection.QuerySingleOrDefaultAsync<Role>(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching role by ID {RoleId}.", roleId);
                throw new RepositoryException("Error occurred while retrieving role.", ex);
            }
        }

        public async Task<string> GetRoleForUserAsync(int userId)
        {
            const string query = "SELECT Role FROM Users WHERE Id = @UserId";
            var parameters = new { UserId = userId };
            try
            {
                return await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { UserId = userId }) ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching role for user ID {UserId}.", userId);
                throw new RepositoryException("Error occurred while retrieving user role.", ex);
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            const string query = @"SELECT r.Name FROM Roles r LEFT JOIN UserRoles ur ON r.Id = ur.RoleId WHERE ur.UserId = @UserId";
            var parameters = new { UserId = userId };
            try
            {
                var roles = await _dbConnection.QueryAsync<string>(query, parameters);
                if (roles == null || !roles.Any()) throw new RepositoryException("No roles found for the given user.");
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
            const string query = @"SELECT * FROM Roles";
            try
            {
                var roles = await _dbConnection.QueryAsync<Role>(query);
                if (roles == null || !roles.Any()) throw new RepositoryException("No roles found.");
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
            if (updates.Count == 0) throw new RepositoryException("No Name and Description provided for update.");

            var sql = $"UPDATE Roles SET {string.Join(", ", updates)} WHERE Id = @Id;";

            try
            {
                var result = await _dbConnection.ExecuteAsync(sql, parameters);
                if (result <= 0) throw new RepositoryException("Failed to update the role.");
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
