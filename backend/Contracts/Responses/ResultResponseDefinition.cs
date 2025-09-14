namespace JobifyEcom.Contracts.Responses;

/// <summary>
/// Standardized success model for consistent API responses.
/// </summary>
/// <param name="Id">Unique machine-readable success code.</param>
/// <param name="Title">Short, human-readable success message.</param>
/// <param name="Details">Optional list of additional context.</param>
public record ResultResponseDefinition(string Id, string Title, string[] Details)
	: ResponseDefinition(Id, Title, Details);
