namespace JobifyEcom.DTOs.User;

/// <summary>
/// Search and filter options for user profile listings.
/// </summary>
public class ProfileFilterRequest
{
	/// <summary>
	/// Search term to match against user profile fields (e.g., name, email).
	/// </summary>
	public string? SearchTerm { get; set; }

	/// <summary>
	/// The property name to sort by (e.g., "Name", "CreatedAt").
	/// Defaults to CreatedAt if not provided.
	/// </summary>
	public string? SortBy { get; set; }

	/// <summary>
	/// Whether sorting should be in descending order.
	/// </summary>
	public bool SortDescending { get; set; } = false;
}
