namespace Voyara.API.Extensions
{
    using System.Security.Claims;
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("nameid")
                ?? throw new UnauthorizedAccessException("User ID not found in token");

            return Guid.Parse(claim);
        }

        public static string GetEmail(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public static string GetRole(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.GetRole() == "Admin";
    }
}
