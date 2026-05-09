using System.Security.Claims;

namespace ChatApp.API.Extension
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst("sub")?.Value;

            if (value == null)
                throw new UnauthorizedAccessException("User ID claim not found");

            return Guid.Parse(value);
        }
    }
}
