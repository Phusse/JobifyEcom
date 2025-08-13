using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents the information required to reset a user's password
/// using a valid password reset token.
/// </summary>
public class PasswordResetRequest
{
	/// <summary>
	/// The unique, time-sensitive token provided to the user
	/// to authorize the password reset.
	/// </summary>
	[Required]
	public string? Token { get; set; }

	/// <summary>
	/// The new password the user wishes to set for their account.
	/// Must meet the system's password policy requirements.
	/// </summary>
	[Required]
	public string? NewPassword { get; set; }
}
