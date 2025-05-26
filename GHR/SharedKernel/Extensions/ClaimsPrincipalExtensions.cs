namespace GHR.SharedKernel.Extensions
{
    using System.Security.Claims;
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user) =>
            user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

        public static string? GetUsername(this ClaimsPrincipal user) =>
            user?.FindFirst(ClaimTypes.Name)?.Value ?? user?.Identity?.Name;

        public static string? GetEmail(this ClaimsPrincipal user) =>
             user?.FindFirst(ClaimTypes.Email)?.Value;

        public static string? GetRole(this ClaimsPrincipal user) =>
             user?.FindFirst(ClaimTypes.Role)?.Value;

        public static string? GetGivenName(this ClaimsPrincipal user) =>
              user?.FindFirst(ClaimTypes.GivenName)?.Value;

        public static string? GetSurname(this ClaimsPrincipal user) =>
             user?.FindFirst(ClaimTypes.Surname)?.Value;


        public static string? GetFullName(this ClaimsPrincipal user)
        {
            var givenName = user.GetGivenName();
            var surname = user.GetSurname();
            return $"{givenName} {surname}".Trim();
        }

        public static string? GetPhoneNumber(this ClaimsPrincipal user) =>
            user?.FindFirst(ClaimTypes.MobilePhone)?.Value ?? user?.FindFirst("phone_number")?.Value;


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
    }
}
