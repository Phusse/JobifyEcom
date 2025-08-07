namespace JobifyEcom.Exceptions;

/// <summary>
/// Represents an exception for conflicting operations (HTTP 409).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConflictException"/> class.
/// </remarks>
/// <param name="message">The error message. Defaults to "Conflict occurred".</param>
/// <param name="errors">An optional list of specific error messages.</param>
public class ConflictException(string message = "Conflict occurred.", List<string>? errors = null) : AppException(StatusCodes.Status409Conflict, message, errors) { }
