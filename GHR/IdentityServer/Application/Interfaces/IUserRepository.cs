namespace IdentityServer.Application.Interfaces
{ 
    using IdentityServer.Domain.Models;

    public interface IUserRepository
    { 
        Task<User> CreateUserAsync(User user, string password); 
        Task<IEnumerable<User>> GetUsersAsync(); 
        Task<User> UpdateUserAsync(User user); 
        Task<bool> DeleteUserAsync(int userId); 
        Task<User?> GetUserByIdAsync(int userId); 
        Task<User?> GetUserByEmailAsync(string email); 
        Task<bool> CheckPasswordAsync(int userId, string passwordHash); 
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
    }

}
