namespace JobifyEcom.DTOs.User;

/// <summary>
/// Search and filter options for user profile listings.
/// </summary>
public class ProfileFilterRequest
{
	/// <summary>
	/// Search term to match against user profile fields (e.g., name, userId).
	/// </summary>
	public string? SearchTerm { get; set; }

	/// <summary>
	/// The property name to sort by (e.g., "Name", "Id").
	/// </summary>
	public string? SortBy { get; set; }

	/// <summary>
	/// Whether sorting should be in descending order.
	/// </summary>
	public bool SortDescending { get; set; } = false;
}
