namespace JobifyEcom.DTOs.Metadata;

/// <summary>
/// Represents a collection of values for a specific enum type,
/// used for exposing enum metadata to API clients.
/// </summary>
public class EnumSetResponse
{
	/// <summary>
	/// The name of the enum type (e.g., <c>"SystemRole"</c>, <c>"JobStatus"</c>).
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// The list of values available in the enum, including their keys and display names.
	/// </summary>
	public required IReadOnlyList<EnumOptionResponse> Values { get; init; }
}
