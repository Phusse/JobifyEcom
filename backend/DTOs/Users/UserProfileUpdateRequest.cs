using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs.Users;

/// <summary>
/// Represents the request to update a user's profile.
/// All fields are optional — only non-null values will be updated.
/// </summary>
public class UserProfileUpdateRequest
{
	/// <summary>
	/// The new name for the user.
	/// Must be between 2 and 100 characters if provided.
	/// </summary>
	[MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
	[StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
	public string? Name { get; set; }

	/// <summary>
	/// The new bio for the user.
	/// Maximum length is 250 characters if provided.
	/// </summary>
	[StringLength(250, ErrorMessage = "Bio cannot exceed 250 characters.")]
	public string? Bio { get; set; }
}
