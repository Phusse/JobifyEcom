using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Enums;

/// <summary>
/// Defines the available fields that can be used to sort user profile search results.
/// </summary>
public enum UserSortField
{
	/// <summary>
	/// Sort by the date and time when the user account was created.
	/// </summary>
	[Display(Name = "Date Created")]
	CreatedAt,

	/// <summary>
	/// Sort by the user’s display name.
	/// </summary>
	[Display(Name = "Display Name")]
	Name,

	/// <summary>
	/// Sort by the unique identifier (GUID) of the user account.
	/// </summary>
	[Display(Name = "User ID")]
	Id,
}
