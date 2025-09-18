namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the response returned after a successful user registration.
/// </summary>
public class RegisterResponse
{
	/// <summary>
	/// The unique confirmation link that the user must visit to verify and activate their account.
	/// </summary>
	public required string ConfirmationLink { get; set; }
}
