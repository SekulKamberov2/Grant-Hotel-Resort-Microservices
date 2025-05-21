namespace IdentityServer.Application.Interfaces
{
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.Models; 
    public interface IUserManager
    {
        Task<IdentityResult<IEnumerable<User>>> GetAllUsersAsync(); 
        Task<IdentityResult<User>> CreateAsync(User user);
        Task<IdentityResult<User>> UpdateAsync(User user);
        Task<IdentityResult<bool>> DeleteAsync(int userId); 
        Task<IdentityResult<User>> FindByIdAsync(int userId); 
        Task<IdentityResult<User?>> FindByEmailAsync(string email); 
        Task<IdentityResult<bool>> ResetPasswordAsync(int Id, string newPassword);
        Task<IdentityResult<User>> ValidateUserAsync(string username, string password); 
    }
}
