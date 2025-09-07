namespace JobifyEcom.Exceptions;

/// <summary>
/// Represents an exception for failed validation (HTTP 400).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationException"/> class.
/// </remarks>
/// <param name="message">The error message. Defaults to "Validation failed".</param>
/// <param name="errors">An optional list of validation errors.</param>
public class ValidationException(
	string message = "Some information you provided is not valid.",
	List<string>? errors = null
) : AppException(StatusCodes.Status400BadRequest, message, errors);
