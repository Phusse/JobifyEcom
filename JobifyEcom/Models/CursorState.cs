using JobifyEcom.DTOs.User;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

/// <summary>
/// Represents the state of a cursor used for paginating user search results.
/// This state is serialized, protected, and passed back to the client so
/// pagination can continue from the last known position.
/// </summary>
public class CursorState
{
	/// <summary>
	/// The <see cref="DateTime"/> value of the last user’s creation timestamp
	/// from the previous page. Used to determine the starting point for the next page.
	/// </summary>
	public required DateTime LastCreatedAt { get; set; }

	/// <summary>
	/// The unique identifier (<see cref="Guid"/>) of the last user from the previous page.
	/// Acts as a tiebreaker when multiple users share the same <see cref="LastCreatedAt"/>.
	/// </summary>
	public required Guid LastId { get; set; }

	/// <summary>
	/// The field by which results are being sorted (e.g., Name, Id, CreatedAt).
	/// </summary>
	public required UserSortField SortBy { get; set; }

	/// <summary>
	/// Indicates whether the sorting is in descending order.
	/// </summary>
	public required bool SortDescending { get; set; }

	/// <summary>
	/// The search and filter criteria that were applied when generating this cursor.
	/// Ensures pagination continues with the same filters applied.
	/// </summary>
	public required ProfileFilterRequest Filter { get; set; }

	/// <summary>
	/// Tracks how many times this cursor has been used to request subsequent pages.
	/// Helps prevent infinite scrolling or excessive depth.
	/// </summary>
	public required int Depth { get; set; }
}
