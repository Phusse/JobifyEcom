using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents the public profile information of a user,
/// excluding sensitive authentication details.
/// </summary>
public class UserProfileResponse
{
	/// <summary>
	/// The unique identifier of the user.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// The full name of the user.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The user's email address.
	/// </summary>
	public required string Email { get; set; }

	/// <summary>
	/// A short biography or description for the user.
	/// </summary>
	public required string Bio { get; set; }

	/// <summary>
	/// The roles assigned to the user.
	/// </summary>
	public required List<SystemRole> Roles { get; set; }

	/// <summary>
	/// The UTC date and time when the user account was created.
	/// </summary>
	public required DateTime CreatedAt { get; set; }
}
