using JobifyEcom.Enums;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
using JobifyEcom.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobifyEcom.Services;

/// <summary>
/// Provides functionality for generating and validating JSON Web Tokens (JWT).
/// </summary>
/// <remarks>
/// Uses application configuration values (<c>JwtSettings</c>) for
/// <c>SecretKey</c>, <c>Issuer</c>, and <c>Audience</c>.
/// </remarks>
internal class JwtTokenService(IConfiguration config)
{
	private readonly IConfiguration _config = config;

	/// <summary>
	/// Generates a signed JWT token for a given user and token type.
	/// </summary>
	/// <param name="user">The user for whom the token is generated.</param>
	/// <param name="expiry">The lifetime of the token.</param>
	/// <param name="tokenType">Specifies whether this is an Access or Refresh token.</param>
	/// <returns>A signed JWT token string.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if JWT configuration values (<c>SecretKey</c>, <c>Issuer</c>, <c>Audience</c>) are missing.
	/// </exception>
	/// <remarks>
	/// The generated token contains:
	/// <list type="bullet">
	/// <item><description><see cref="ClaimTypes.NameIdentifier"/> – user's unique ID.</description></item>
	/// <item><description>email – user's email address.</description></item>
	/// <item><description>role – user's role.</description></item>
	/// <item><description>security_stamp – unique GUID for token invalidation.</description></item>
	/// <item><description>TokenType – indicates whether it's Access or Refresh.</description></item>
	/// </list>
	/// </remarks>
	internal string GenerateToken(User user, TimeSpan expiry, TokenType tokenType)
	{
		IConfigurationSection jwtSettings = _config.GetSection("JwtSettings");
		string? secretKey = jwtSettings["SecretKey"];
		string? issuer = jwtSettings["Issuer"];
		string? audience = jwtSettings["Audience"];

		if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
		{
			throw new InvalidOperationException("JWT configuration is missing or incomplete.");
		}

		List<Claim> claims =
		[
			new Claim(AppClaimTypes.UserId, user.Id.ToString()),
			new Claim(AppClaimTypes.Email, user.Email),
			new Claim(AppClaimTypes.SecurityStamp, user.SecurityStamp.ToString()),
			new Claim(AppClaimTypes.TokenType, tokenType.ToString()),
		];

		foreach (SystemRole role in user.GetUserRoles())
		{
			claims.Add(new Claim(AppClaimTypes.Role, role.ToString()));
		}

		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
		SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

		JwtSecurityToken token = new(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.Add(expiry),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	/// <summary>
	/// Validates a JWT against configuration settings and returns its claims principal if valid.
	/// </summary>
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
	internal ClaimsPrincipal? ValidateToken(string token, TokenType? expectedType = null)
	{
		IConfigurationSection jwtSettings = _config.GetSection("JwtSettings");
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
				string? typeClaim = principal.GetTokenType();

				if (!Enum.TryParse(typeClaim, ignoreCase: true, out TokenType actualType) || actualType != expectedType.Value)
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
}
