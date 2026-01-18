using System.Security.Claims;

namespace Jobify.Ecom.Api.Extensions.Claims;

internal static class ClaimsPrincipalExtensions
{
    public static Guid? GetInternalUserId(this ClaimsPrincipal user)
    {
        string? claimValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(claimValue, out Guid parsedId))
            return parsedId;

        return null;
    }
}
