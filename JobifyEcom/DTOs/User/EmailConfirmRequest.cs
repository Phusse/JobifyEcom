using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents the request needed to confirm a user's email address.
/// </summary>
public class EmailConfirmRequest
{
	/// <summary>
	/// The email address to confirm.
	/// This must match the email used during registration.
	/// </summary>
	[Required(ErrorMessage = "Please enter your email address.")]
	[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
	public string? Email { get; set; }

	/// <summary>
	/// The unique confirmation token sent to the user's email.
	/// </summary>
	[Required(ErrorMessage = "Email confirmation token is required. Please check your email for the link.")]
	public Guid? Token { get; set; }
}
