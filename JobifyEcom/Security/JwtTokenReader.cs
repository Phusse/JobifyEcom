using System.IdentityModel.Tokens.Jwt;

namespace JobifyEcom.Security;

/// <summary>
/// Reads and extracts claims and metadata from JSON Web Tokens (JWT).
/// </summary>
public class JwtTokenReader
{
	/// <summary>
	/// Gets the expiration date and time (UTC) from a JWT token.
	/// </summary>
	/// <param name="token">The JWT token string.</param>
	/// <returns>The token's expiration date and time in UTC.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if the token does not contain a valid <c>exp</c> claim.
	/// </exception>
	/// <remarks>
	/// The <c>exp</c> claim is stored as seconds since the Unix epoch (UTC).
	/// </remarks>
	public static DateTime GetExpiryFromToken(string token)
	{
		JwtSecurityTokenHandler handler = new();
		JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

		string? expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

		if (expClaim is null || !long.TryParse(expClaim, out long expSeconds))
		{
			throw new InvalidOperationException("The provided token is missing a valid expiration claim, so its expiry time cannot be determined.");
		}

		return DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
	}
}
