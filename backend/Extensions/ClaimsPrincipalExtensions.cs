using System.Security.Claims;
using JobifyEcom.Security;

namespace JobifyEcom.Extensions;

/// <summary>
/// Extension methods for extracting strongly-typed values from a <see cref="ClaimsPrincipal"/>.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal user)
    {
        /// <summary>
        /// Gets the authenticated user's ID from claims.
        /// </summary>
        internal Guid? GetUserId()
        {
            string? value = user.FindFirstValue(AppClaimTypes.UserId);
            return Guid.TryParse(value, out Guid id) ? id : null;
        }

        /// <summary>
        /// Gets the authenticated user's email from claims.
        /// </summary>
        internal string? GetEmail()
        {
            return user.FindFirstValue(AppClaimTypes.Email);
        }

        /// <summary>
        /// Gets the authenticated user's roles from claims.
        /// </summary>
        /// <returns>A list of roles assigned to the user.</returns>
        internal IReadOnlyList<string> GetRoles()
        {
            return [.. user.FindAll(AppClaimTypes.Role).Select(c => c.Value)];
        }

        /// <summary>
        /// Gets the security stamp for the user's current session from claims.
        /// </summary>
        internal Guid? GetSecurityStamp()
        {
            string? value = user.FindFirstValue(AppClaimTypes.SecurityStamp);
            return Guid.TryParse(value, out Guid stamp) ? stamp : null;
        }

        /// <summary>
        /// Gets the token type (Access or Refresh) from claims.
        /// </summary>
        internal string? GetTokenType()
        {
            return user.FindFirstValue(AppClaimTypes.TokenType);
        }

        /// <summary>
        /// Gets all claims as a dictionary for easier debugging/logging.
        /// Preserves multiple values for the same claim type (e.g., roles).
        /// </summary>
        internal Dictionary<string, List<string>> GetAllClaims()
        {
            return user.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => c.Value ?? string.Empty).ToList()
                );
        }

        /// <summary>
        /// Logs all claims to console (useful for debugging).
        /// </summary>
        internal void LogClaims(string? prefix = null)
        {
            prefix ??= "[Claims]";

            foreach (Claim claim in user.Claims)
            {
                Console.WriteLine($"{prefix} {claim.Type}: {claim.Value}");
            }
        }
    }
}
