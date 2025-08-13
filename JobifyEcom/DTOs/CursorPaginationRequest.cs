using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.DTOs;

/// <summary>
/// Represents a generic paginated request with cursor-based pagination and optional filtering.
/// </summary>
public class CursorPaginationRequest<T>
{
	/// <summary>
	/// Cursor token for the last seen record. Null for the first page.
	/// </summary>
	public string? Cursor { get; set; }

	/// <summary>
	/// The maximum number of items per page. Default is 20.
	/// </summary>
	[Range(1, 20)]
	public int PageSize { get; set; } = 20;

	/// <summary>
	/// Maximum number of pages allowed for this query.
	/// Helps prevent excessive data traversal in one request.
	/// </summary>
	[Range(1, 20)]
	public int MaxPages { get; set; } = 20;

	/// <summary>
	/// Optional filter object for entity-specific queries.
	/// </summary>
	public T? Filter { get; set; }
}
