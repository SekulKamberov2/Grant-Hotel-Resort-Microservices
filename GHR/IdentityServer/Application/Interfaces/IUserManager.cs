namespace IdentityServer.Application.Interfaces
{
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.DTOs;
    using IdentityServer.Domain.Models; 
    public interface IUserManager
    {
        Task<IdentityResult<IEnumerable<User>>> GetAllUsersAsync(); 
        Task<IdentityResult<UserDTO>> CreateAsync(UserDTO user);
        Task<IdentityResult<UserDTO>> UpdateAsync(UserDTO user);
        Task<IdentityResult<bool>> DeleteAsync(int userId); 
        Task<IdentityResult<UserDTO>> FindByIdAsync(int userId); 
        Task<IdentityResult<UserDTO?>> FindByEmailAsync(string email); 
        Task<IdentityResult<bool>> ResetPasswordAsync(int Id, string newPassword);
        Task<IdentityResult<User>> ValidateUserAsync(string username, string password); 
    }
}
