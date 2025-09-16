using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.Users;

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
	[Required(ErrorMessage = "Password reset token is required. Please check your email for the link.")]
	public Guid Token { get; set; } = Guid.Empty;

	/// <summary>
	/// The new password the user wishes to set for their account.
	/// Must meet the system's password policy requirements.
	/// </summary>
	[Required(ErrorMessage = "Please enter your new password.")]
	[MinLength(6, ErrorMessage = "Your new password must be at least 6 characters long.")]
	public string? NewPassword { get; set; } = string.Empty;
}
