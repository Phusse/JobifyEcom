namespace JobifyEcom.Exceptions;

/// <summary>
/// Represents a structured, centralized error used across the application.
/// Includes a unique code, HTTP status, title, and optional details.
/// </summary>
/// <param name="Code">Unique, machine-friendly error code (e.g. "AUTH_INVALID_SESSION").</param>
/// <param name="HttpStatus">The HTTP status code to return for this error.</param>
/// <param name="Title">Short, user-facing error message.</param>
/// <param name="Details">Optional list of extra details (e.g. validation messages).</param>
public record ErrorDefinition(
	string Code,
	int HttpStatus,
	string Title,
	string[]? Details = null
);
