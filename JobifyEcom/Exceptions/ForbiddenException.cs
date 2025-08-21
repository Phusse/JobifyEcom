namespace JobifyEcom.Exceptions;

/// <summary>
/// Represents an exception for forbidden actions (HTTP 403).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ForbiddenException"/> class.
/// </remarks>
/// <param name="message">The error message. Defaults to "You do not have permission to perform this action."</param>
/// <param name="errors">An optional list of error details.</param>
public class ForbiddenException(
	string message = "You do not have permission to perform this action.",
	List<string>? errors = null
) : AppException(StatusCodes.Status403Forbidden, message, errors);