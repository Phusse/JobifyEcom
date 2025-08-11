using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the request to refresh authentication tokens.
/// </summary>
public class RefreshTokenRequest
{
	/// <summary>
	/// Gets or sets the refresh token provided by the client.
	/// </summary>
	[Required]
	public string? RefreshToken { get; set; }
}
