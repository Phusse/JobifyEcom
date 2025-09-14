namespace JobifyEcom.Contracts.Responses;

/// <summary>
/// Standardized error model for consistent API responses.
/// </summary>
/// <param name="Id">Unique machine-readable error code.</param>
/// <param name="HttpStatus">HTTP status code to return.</param>
/// <param name="Title">Short, human-readable error message.</param>
/// <param name="Details">Optional list of additional context, such as validation messages.</param>
public record ErrorResponseDefinition(string Id, int HttpStatus, string Title, string[] Details)
	: ResponseDefinition(Id, Title, Details);
