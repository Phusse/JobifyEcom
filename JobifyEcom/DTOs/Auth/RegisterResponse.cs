namespace JobifyEcom.DTOs.Auth;

/// <summary>
/// Represents the response returned after a successful user registration.
/// </summary>
public class RegisterResponse
{
	/// <summary>
	/// Gets or sets the email confirmation link that the user needs to visit to activate their account.
	/// </summary>
	public required string ConfirmationLink { get; set; }
}
