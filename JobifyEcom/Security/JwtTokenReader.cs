using System.IdentityModel.Tokens.Jwt;

namespace JobifyEcom.Security;

/// <summary>
/// Provides functionality for validating and extracting information from JSON Web Tokens (JWT).
/// </summary>
/// <remarks>
/// This class handles token signature validation, issuer/audience checks,
/// and optional enforcement of a specific <see cref="TokenType"/> claim.
/// It can also read expiry dates from tokens without validating them.
/// </remarks>
public class JwtTokenReader
{
	/// <summary>
	/// Extracts the expiry date from a JWT without performing validation.
	/// </summary>
	/// <param name="token">The JWT string.</param>
	/// <returns>
	/// The expiry date in UTC if available; otherwise <c>null</c>.
	/// </returns>
	public static DateTime? GetExpiryFromToken(string token)
	{
		JwtSecurityTokenHandler handler = new();

		if (handler.ReadToken(token) is not JwtSecurityToken jwtToken)
		{
			return null;
		}

		string? expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

		if (expClaim is null || !long.TryParse(expClaim, out long expUnix))
		{
			return null;
		}

		return DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
	}
}
