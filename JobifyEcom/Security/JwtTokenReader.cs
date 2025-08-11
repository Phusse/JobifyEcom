using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
	/// Validates a JWT against configuration settings and returns its claims principal if valid.
	/// </summary>
	/// <param name="config">The application configuration containing <c>JwtSettings</c> values.</param>
	/// <param name="token">The JWT string to validate.</param>
	/// <param name="expectedType">
	/// Optional token type (<see cref="TokenType.Access"/> or <see cref="TokenType.Refresh"/>) to enforce.
	/// If provided, the token must contain a matching <c>token_type</c> claim.
	/// </param>
	/// <returns>
	/// A <see cref="ClaimsPrincipal"/> containing the token's claims if valid; otherwise <c>null</c>.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if required JWT configuration values (<c>SecretKey</c>, <c>Issuer</c>, <c>Audience</c>) are missing.
	/// </exception>
	public static ClaimsPrincipal? ValidateToken(IConfiguration config, string token, TokenType? expectedType = null)
	{
		IConfigurationSection jwtSettings = config.GetSection("JwtSettings");
		string? secretKey = jwtSettings["SecretKey"];
		string? issuer = jwtSettings["Issuer"];
		string? audience = jwtSettings["Audience"];

		if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
		{
			throw new InvalidOperationException("JWT configuration is missing or incomplete.");
		}

		try
		{
			JwtSecurityTokenHandler tokenHandler = new();
			byte[] key = Encoding.UTF8.GetBytes(secretKey);

			TokenValidationParameters validationParams = new()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = issuer,
				ValidateAudience = true,
				ValidAudience = audience,
				ClockSkew = TimeSpan.Zero,
			};

			ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);

			if (validatedToken is not JwtSecurityToken jwtToken || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}

			// Enforce token type if requested
			if (expectedType.HasValue)
			{
				string? typeClaim = principal.FindFirst("token_type")?.Value;

				if (!Enum.TryParse<TokenType>(typeClaim, ignoreCase: true, out TokenType actualType) || actualType != expectedType.Value)
				{
					return null;
				}
			}

			return principal;
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Extracts the expiry date from a JWT without performing validation.
	/// </summary>
	/// <param name="token">The JWT string.</param>
	/// <returns>
	/// The expiry date in UTC if available; otherwise <c>null</c>.
	/// </returns>
	public static DateTime? GetExpiryFromToken(string token)
	{
		JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

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
