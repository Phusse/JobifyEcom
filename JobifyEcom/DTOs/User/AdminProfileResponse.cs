using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents detailed information about a user for administrative purposes.
/// Excludes sensitive authentication details like password hashes or tokens.
/// </summary>
public class AdminProfileResponse : ProfileResponse
{
	/// <summary>
	/// Indicates whether the user's email has been confirmed.
	/// </summary>
	public bool IsEmailConfirmed { get; set; }

	/// <summary>
	/// Indicates whether the user account is currently locked.
	/// </summary>
	public bool IsLocked { get; set; }

	/// <summary>
	/// The UTC datetime when the account was locked, if applicable.
	/// </summary>
	public DateTime? LockedAt { get; set; }

	/// <summary>
	/// The UTC datetime when the user was last updated.
	/// </summary>
	public DateTime UpdatedAt { get; set; }

	/// <summary>
	/// Whether the user has a worker profile.
	/// </summary>
	public bool IsWorker { get; set; }
}
