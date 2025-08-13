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
	/// The current page index (starting at 1).
	/// </summary>
	public required int CurrentPage { get; set; }

	/// <summary>
	/// The total number of pages available for this query (respecting any max limit).
	/// </summary>
	public int? TotalPages { get; set; }

	/// <summary>
	/// The list of items for the current page.
	/// </summary>
	public required IReadOnlyList<T> Items { get; init; } = [];
}
