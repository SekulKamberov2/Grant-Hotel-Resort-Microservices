namespace IdentityServer.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Dapper;
    using IdentityServer.Application.Exceptions;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Domain.Exceptions;
    using IdentityServer.Domain.Models;

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<User> CreateUserAsync(User user, string password)
        {
            using var connection = CreateConnection();
            const string query = @"
                INSERT INTO Users (Email, UserName, PhoneNumber, PasswordHash) 
                OUTPUT INSERTED.Id, INSERTED.Email, INSERTED.UserName, INSERTED.PhoneNumber, INSERTED.DateCreated
                VALUES (@Email, @UserName, @PhoneNumber, @PasswordHash);";

            var parameters = new { user.Email, user.UserName, user.PhoneNumber, PasswordHash = password };

            try
            {
                var insertedUser = await connection.QuerySingleOrDefaultAsync<User>(query, parameters);
                if (insertedUser == null)
                    throw new RepositoryException("User creation failed.");

                return insertedUser;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during user creation.");
                throw new RepositoryException("Error creating user from the database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user creation.");
                throw new RepositoryException("Unexpected error occurred while creating user.", ex);
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            using var connection = CreateConnection();
            const string query = @"
                UPDATE Users 
                SET PhoneNumber = @PhoneNumber, Email = @Email 
                WHERE Id = @Id;

                SELECT Id, UserName, PhoneNumber, Email, DateCreated 
                FROM Users 
                WHERE Id = @Id;";

            var parameters = new { user.PhoneNumber, user.Email, user.Id };

            try
            {
                var updatedUser = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
                if (updatedUser == null)
                    throw new NotFoundException($"User with ID {user.Id} not found after update.");

                return updatedUser;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during user update.");
                throw new RepositoryException("Error occurred while updating user.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user update.");
                throw new RepositoryException("Unexpected error occurred while updating user.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            using var connection = CreateConnection();
            const string sql = @"
                SELECT u.Id, u.UserName, u.Email, u.PhoneNumber, u.DateCreated, r.Name AS RoleName
                FROM Users u
                LEFT JOIN UserRoles ur ON u.Id = ur.UserId
                LEFT JOIN Roles r ON ur.RoleId = r.Id";

            try
            {
                var userDictionary = new Dictionary<int, User>();

                await connection.QueryAsync<User, string, User>(
                    sql,
                    (user, roleName) =>
                    {
                        if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                        {
                            existingUser = user;
                            existingUser.Roles = new List<string>();
                            userDictionary.Add(existingUser.Id, existingUser);
                        }

                        if (!string.IsNullOrEmpty(roleName))
                            existingUser.Roles.Add(roleName);

                        return existingUser;
                    },
                    splitOn: "RoleName"
                );

                return userDictionary.Values;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during fetching users with roles.");
                throw new RepositoryException("Error occurred while fetching users with roles.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during fetching users with roles.");
                throw new RepositoryException("Unexpected error occurred while fetching users.", ex);
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var connection = CreateConnection();
            const string query = "DELETE FROM Users WHERE Id = @UserId";
            var parameters = new { UserId = userId };

            try
            {
                var affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during user deletion.");
                throw new RepositoryException("An error occurred while deleting the user.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user deletion.");
                throw new RepositoryException("Unexpected error occurred while deleting the user.", ex);
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = CreateConnection();
            const string query = "SELECT * FROM Users WHERE Email = @Email";
            var parameters = new { Email = email };

            try
            {
                return await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during fetching user by email.");
                throw new RepositoryException("Error fetching user by email.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during fetching user by email.");
                throw new RepositoryException("Unexpected error occurred while fetching user by email.", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            using var connection = CreateConnection();
            const string query = "SELECT * FROM Users WHERE Id = @UserId";
            var parameters = new { UserId = userId };

            try
            {
                var result = await connection.QuerySingleOrDefaultAsync<User>(query, parameters);
                if (result == null)
                    throw new NotFoundException($"User with ID {userId} was not found.");
                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during fetching user by ID.");
                throw new RepositoryException("Error fetching user by ID.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during fetching user by ID.");
                throw new RepositoryException("Unexpected error occurred while fetching user by ID.", ex);
            }
        }

        public async Task<bool> CheckPasswordAsync(int userId, string passwordHash)
        {
            using var connection = CreateConnection();
            const string query = "SELECT PasswordHash FROM Users WHERE Id = @Id";
            var parameters = new { Id = userId };

            try
            {
                var storedHash = await connection.QuerySingleOrDefaultAsync<string>(query, parameters);
                return storedHash == passwordHash;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during password check.");
                throw new RepositoryException("An error occurred while checking the password.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password check.");
                throw new RepositoryException("Unexpected error occurred while checking the password.", ex);
            }
        }

        public async Task<string> GetRoleForUserAsync(int userId)
        {
            using var connection = CreateConnection();
            const string query = @"
                SELECT r.Name
                FROM Roles r
                INNER JOIN UserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId";
            var parameters = new { UserId = userId };

            try
            {
                return await connection.QueryFirstOrDefaultAsync<string>(query, parameters) ?? string.Empty;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during fetching user role.");
                throw new RepositoryException("Error occurred while retrieving user role.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during fetching user role.");
                throw new RepositoryException("Unexpected error occurred while retrieving user role.", ex);
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            using var connection = CreateConnection();
            const string updateQuery = "UPDATE Users SET PasswordHash = @NewPassword WHERE Id = @UserId";
            var parameters = new { UserId = userId, NewPassword = newPassword };

            try
            {
                return await connection.ExecuteAsync(updateQuery, parameters) > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during password reset.");
                throw new RepositoryException("Error occurred while resetting the password.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password reset.");
                throw new RepositoryException("Unexpected error occurred while resetting the password.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetUserProfilesByIds(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return Enumerable.Empty<User>();
            using var connection = CreateConnection();
            var sql = @"SELECT Id, UserName, Email, PhoneNumber, DateCreated FROM Users WHERE Id IN @Ids;";
            return await connection.QueryAsync<User>(sql, new { Ids = ids.ToArray() });
        }
    }
}
