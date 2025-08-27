using JobifyEcom.Enums;

namespace JobifyEcom.DTOs.User;

/// <summary>
/// Represents search and filter options for querying user profiles.
/// </summary>
public class UserProfileFilterRequest
{
	/// <summary>
	/// Search term to match against user profile fields (e.g., ID, email, or name).
	/// Interpretation depends on <see cref="SearchBy"/>.
	/// </summary>
	public string? SearchTerm { get; set; }

	/// <summary>
	/// Field to search by (ID, email, name).
	/// Defaults to <see cref="UserSearchField.Auto"/> which auto-detects based on the input.
	/// </summary>
	public UserSearchField SearchBy { get; set; } = UserSearchField.Auto;

	/// <summary>
	/// Field to sort the results by (e.g., CreatedAt, Name, Id).
	/// Defaults to <see cref="UserSortField.CreatedAt"/>.
	/// </summary>
	public UserSortField SortBy { get; set; } = UserSortField.CreatedAt;

	/// <summary>
	/// Whether the results should be sorted in descending order.
	/// Defaults to <c>false</c> (ascending).
	/// </summary>
	public bool SortDescending { get; set; } = false;
}
