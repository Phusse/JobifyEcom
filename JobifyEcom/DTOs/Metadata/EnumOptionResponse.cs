namespace JobifyEcom.DTOs.Metadata;

/// <summary>
/// Represents a single option in an enum set returned by the API.
/// </summary>
public class EnumOptionResponse
{
	/// <summary>
	/// The enum key name as defined in code (used as the value in the DB).
	/// </summary>
	public required string Key { get; init; }

	/// <summary>
	/// A human-friendly display name for the enum value.
	/// Falls back to <see cref="Key"/> if no <c>[Display(Name = "...")]</c> attribute is set.
	/// </summary>
	public required string DisplayName { get; init; }
}
