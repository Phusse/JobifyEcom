using JobifyEcom.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobifyEcom.Security;

/// <summary>
/// Generates signed JSON Web Tokens (JWT) for authenticated users.
/// </summary>
/// <remarks>
/// Uses application configuration values (<c>JwtSettings</c>) for
/// <c>SecretKey</c>, <c>Issuer</c>, and <c>Audience</c>.
/// </remarks>
public class JwtTokenGenerator(IConfiguration config)
{
	private readonly IConfiguration _config = config;

	/// <summary>
	/// Gets the configuration instance used to read JWT settings.
	/// </summary>
	public IConfiguration Config => _config;

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
	public string GenerateToken(User user, TimeSpan expiry, TokenType tokenType)
	{
		IConfigurationSection jwtSettings = _config.GetSection("JwtSettings");
		string? secretKey = jwtSettings["SecretKey"];
		string? issuer = jwtSettings["Issuer"];
		string? audience = jwtSettings["Audience"];

		if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
		{
			throw new InvalidOperationException("JWT configuration is missing or incomplete.");
		}

		Claim[] claims =
		[
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim("email", user.Email),
			new Claim("role", user.Role.ToString()),
			new Claim("security_stamp", user.SecurityStamp.ToString()),
			new Claim("token_type", tokenType.ToString()),
		];

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
}
