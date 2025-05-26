namespace GHR.SharedKernel.Extensions
{
    using System.Security.Claims;
    using System.Text.Json;

    using GHR.SharedKernel.Models;
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user) =>
            user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("userId")?.Value;
         
        public static bool TryGetUserIdAsInt(this ClaimsPrincipal user, out int userId)
        {
            userId = 0;
            var id = user?.GetUserId();
            return int.TryParse(id, out userId);
        }

        public static bool TryGetUserIdAsGuid(this ClaimsPrincipal user, out Guid userId)
        {
            userId = Guid.Empty;
            var id = user?.GetUserId();
            return Guid.TryParse(id, out userId);
        }

        public static string? GetFullName(this ClaimsPrincipal user) =>
            user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("userName")?.Value;

        public static string? GetEmail(this ClaimsPrincipal user) =>
             user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("mail")?.Value;

        public static string? GetRole(this ClaimsPrincipal user) =>
             user?.FindFirst(ClaimTypes.Role)?.Value;

        public static string? GetPhoneNumber(this ClaimsPrincipal user) =>
              user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("phone")?.Value;
           
        public static string? GetCustomClaim(this ClaimsPrincipal user, string claimType) =>
             user?.FindFirst(claimType)?.Value;
         
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal user) =>

            user?.FindAll(ClaimTypes.Role)?.Select(r => r.Value) ?? Enumerable.Empty<string>();
         
        public static Dictionary<string, string> GetAllClaims(this ClaimsPrincipal user)
        {
            return user?.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.First().Value)
                ?? new Dictionary<string, string>();
        }

        public static CurrentUser ToCurrentUser(this ClaimsPrincipal user)
        { 
            int userId = user.TryGetUserIdAsInt(out int id) ? id : 0;
            var currentUser = new CurrentUser(
                userId,
                user.GetFullName() ?? "Unauthorized",
                user.GetEmail() ?? "Unauthorized",
                user.GetPhoneNumber() ?? "Unauthorized"); 
            return currentUser;
        } 
    }
}
