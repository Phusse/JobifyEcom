using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the data required to request a new access token
/// using a valid refresh token.
/// </summary>
public class RefreshTokenRequest
{
	/// <summary>
	/// The refresh token issued to the client during authentication.
	/// This token is required to obtain a new access token.
	/// </summary>
	[Required(ErrorMessage = "A refresh token is required to continue. Please log in again if you don't have one.")]
	public string? RefreshToken { get; set; }
}
