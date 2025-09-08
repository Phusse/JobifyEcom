namespace JobifyEcom.DTOs.Users;

/// <summary>
/// Represents a lightweight view of a user's profile,
/// intended for list or summary displays in paginated results.
/// </summary>
public class UserProfileSummaryResponse
{
	/// <summary>
	/// The unique identifier of the user.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// The full name of the user.
	/// </summary>
	public required string Name { get; set; }
}
