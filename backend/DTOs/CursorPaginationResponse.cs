namespace JobifyEcom.DTOs;

/// <summary>
/// Represents a paginated response with cursor-based pagination.
/// </summary>
public class CursorPaginationResponse<T>
{
	/// <summary>
	/// Cursor token for fetching the next page. Null if there are no more results.
	/// </summary>
	public string? NextCursor { get; set; }

	/// <summary>
	/// True if there is another page after this one.
	/// </summary>
	public required bool HasMore { get; set; }

	/// <summary>
	/// The list of items for the current page.
	/// </summary>
	public required IReadOnlyList<T> Items { get; init; } = [];
}
