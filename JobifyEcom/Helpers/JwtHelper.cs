using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobifyEcom.Helpers;

/// <summary>
/// Helper class for generating JSON Web Tokens (JWT) for authenticated users.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JwtHelper"/> class.
/// </remarks>
/// <param name="config">Application configuration for accessing JWT settings.</param>
public class JwtHelper(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    /// <summary>
    /// Generates a signed JWT token with user claims.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="role">The role assigned to the user (e.g., admin, user).</param>
    /// <returns>A signed JWT token as a string.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if required JWT configuration values are missing.
    /// </exception>
    public string GenerateToken(Guid userId, string email, string role)
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
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("email", email),
            new Claim("role", role),
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

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        string tokenString = handler.WriteToken(token);

        return tokenString;
    }
}
