using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.Exceptions;

/// <summary>
/// The base class for all custom application exceptions.
/// Encapsulates an HTTP status code, a unique code, and optionally a list of error messages.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="AppException"/> using an <see cref="ErrorResponseDefinition"/>.
/// </remarks>
/// <param name="error">The predefined error definition.</param>
public class AppException(ErrorResponseDefinition error) : Exception(error.Title)
{
	/// <summary>
	/// Gets the HTTP status code associated with the exception.
	/// </summary>
	public int StatusCode { get; } = error.HttpStatus;

	/// <summary>
	/// Gets the unique identifier for the exception (optional).
	/// </summary>
	public string? Id { get; } = error.Id;

	/// <summary>
	/// Gets the list of detailed error messages, if provided.
	/// </summary>
	public List<string>? Details { get; } = [.. error.Details];
}
