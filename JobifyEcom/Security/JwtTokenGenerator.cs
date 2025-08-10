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
/// This service uses application configuration values for JWT settings such as
/// <c>SecretKey</c>, <c>Issuer</c>, and <c>Audience</c> from <c>JwtSettings</c>.
/// </remarks>
/// <param name="config">The application configuration for accessing JWT settings.</param>
public class JwtTokenGenerator(IConfiguration config)
{
	private readonly IConfiguration _config = config;

	/// <summary>
	/// Generates a signed JWT token containing user claims.
	/// </summary>
	/// <param name="user">The user for whom the token is generated.</param>
	/// <returns>A signed JWT token as a string.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if required JWT configuration values such as <c>SecretKey</c>, <c>Issuer</c>, or <c>Audience</c> are missing.
	/// </exception>
	/// <remarks>
	/// The generated token includes the following claims:
	/// <list type="bullet">
	/// <item><description><see cref="ClaimTypes.NameIdentifier"/>: The user's unique ID.</description></item>
	/// <item><description>email: The user's email address.</description></item>
	/// <item><description>role: The user's role (e.g., Admin, Customer).</description></item>
	/// <item><description>security_stamp: A unique GUID used to validate the token against the user's current security stamp to enable token invalidation.</description></item>
	/// </list>
	/// The token expires after 3 hours from issuance.
	/// </remarks>
	public string GenerateToken(User user)
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
			];

		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
		SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

		JwtSecurityToken token = new(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddHours(3),
			signingCredentials: creds
		);

		JwtSecurityTokenHandler handler = new();
		return handler.WriteToken(token);
	}
}
