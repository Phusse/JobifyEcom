namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents the request to update a user's profile.
/// </summary>
public class ProfileUpdateRequest
{
	/// <summary>
	/// The new name for the user.
	/// </summary>
	public string? Name { get; set; }
}
