using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.Contracts.Results;

/// <summary>
/// Central registry of all standardized application results (non-error outcomes).
/// Each entry provides a unique code, a user-facing title, and optional details.
/// Organized into <c>partial</c> files by domain for easier maintenance,
/// and can be used to represent successful or informational outcomes in <c>ServiceResult</c>.
/// </summary>
internal static partial class ResultCatalog
{
	public static readonly ResultResponseDefinition AllEnumsRetrieved = new(
		Id: "METADATA_ENUMS_RETRIEVED",
		Title: "All enums retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition EnumRetrieved = new(
		Id: "METADATA_ENUM_RETRIEVED",
		Title: "Enum retrieved successfully.",
		Details: []
	);

	internal static readonly ResultResponseDefinition EnumNotFound = new(
		Id: "METADATA_ENUM_NOT_FOUND",
		Title: $"The requested enum could not be found.",
		Details: ["Please check the list of available enums."]
	);
}
