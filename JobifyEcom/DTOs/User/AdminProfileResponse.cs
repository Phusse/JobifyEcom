namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents detailed information about a user for administrative purposes.
/// Excludes sensitive authentication details like password hashes or tokens.
/// </summary>
public class AdminProfileResponse : ProfileResponse
{
	/// <summary>
	/// Indicates whether the user's email has been confirmed. (Admin only).
	/// </summary>
	public required bool IsEmailConfirmed { get; set; }

	/// <summary>
	/// Indicates whether the user account is currently locked. (Admin only).
	/// </summary>
	public required bool IsLocked { get; set; }

	/// <summary>
	/// The UTC datetime when the account was locked, if applicable. (Admin only).
	/// </summary>
	public required DateTime? LockedAt { get; set; }

	/// <summary>
	/// The UTC datetime when the user was last updated. (Admin only).
	/// </summary>
	public required DateTime UpdatedAt { get; set; }
}
