namespace IdentityServer.Application.Interfaces
{
    using IdentityServer.Application.Results;
    using IdentityServer.Domain.Models;
    public interface ITokenService
    {
        IdentityResult<string> GenerateToken(string userId, User user, IEnumerable<string> roles);
    } 
}
