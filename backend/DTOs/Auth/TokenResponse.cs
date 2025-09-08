namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the response returned after a successful login or token refresh.
/// </summary>
public class TokenResponse
{
	/// <summary>
	/// Gets or sets the short-lived access token.
	/// </summary>
	public required string AccessToken { get; set; }

	/// <summary>
	/// Gets or sets the expiration date and time of the access token.
	/// </summary>
	public required DateTime? AccessTokenExpiresAt { get; set; }

	/// <summary>
	/// Gets or sets the long-lived refresh token.
	/// </summary>
	public required string RefreshToken { get; set; }

	/// <summary>
	/// Gets or sets the expiration date and time of the refresh token.
	/// </summary>
	public required DateTime? RefreshTokenExpiresAt { get; set; }
}
