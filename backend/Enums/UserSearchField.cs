using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Defines which field a user search query should match against.
/// </summary>
public enum UserSearchField
{
	/// <summary>
	/// Automatically detects the field to search (GUID → Id, email → Email, else Name).
	/// </summary>
	[Display(Name = "Auto-detect")]
	Auto,

	/// <summary>
	/// Search against the unique user identifier (Guid).
	/// </summary>
	[Display(Name = "User ID")]
	Id,

	/// <summary>
	/// Search against the user’s email address.
	/// </summary>
	[Display(Name = "Email address")]
	Email,

	/// <summary>
	/// Search against the user’s display name.
	/// </summary>
	[Display(Name = "Display name")]
	Name,
}
