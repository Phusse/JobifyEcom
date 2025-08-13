using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents the request to confirm an email address.
/// </summary>
public class EmailConfirmRequest
{
	/// <summary>
	/// The email address to confirm. This should be the same as the one used for registration.
	/// </summary>
	[Required]
	public string? Email { get; set; }

	/// <summary>
	/// The token used to confirm the email address.
	/// </summary>
	[Required]
	public string? Token { get; set; }
}
