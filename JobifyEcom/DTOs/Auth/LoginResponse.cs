using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the response returned after a successful login.
/// </summary>
public class LoginResponse
{
	/// <summary>
	/// Gets or sets the unique identifier of the user.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the full name of the user.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets the user's email address.
	/// </summary>
	public required string Email { get; set; }

	/// <summary>
	/// Gets or sets the authentication token issued to the user.
	/// </summary>
	public required string Token { get; set; }

	/// <summary>
	/// Gets or sets the expiration date and time of the authentication token.
	/// </summary>
	public required DateTime ExpiresAt { get; set; }

	/// <summary>
	/// Gets or sets the role assigned to the user.
	/// </summary>
	public required UserRole Role { get; set; }
}
