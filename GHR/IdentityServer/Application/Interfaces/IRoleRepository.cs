namespace IdentityServer.Application.Interfaces
{
    using IdentityServer.Domain.Models;
    public interface IRoleRepository
    {
        Task<bool> CreateUserRoleAsync(string roleName, string description);
        Task<bool> UpdateUserRoleAsync(int id, string? roleName, string? description);
        Task<bool> DeleteUserRoleAsync(int role);
        Task<Role> FindRoleByIdAsync(int roleId);
        Task<bool> AddUserToRoleAsync(int userId, int role);
        Task<string> GetRoleForUserAsync(int userId);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<IEnumerable<Role>> GetRolesAsync();
    }
}
