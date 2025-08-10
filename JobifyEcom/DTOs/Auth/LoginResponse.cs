namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the response returned after a successful login.
/// </summary>
public class LoginResponse
{
	/// <summary>
	/// Gets or sets the authentication token issued to the user.
	/// </summary>
	public required string Token { get; set; }

	/// <summary>
	/// Gets or sets the expiration date and time of the authentication token.
	/// </summary>
	public required DateTime ExpiresAt { get; set; }
}
