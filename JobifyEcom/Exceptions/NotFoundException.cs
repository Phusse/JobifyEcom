namespace JobifyEcom.Exceptions;

/// <summary>
/// Represents an exception for resources that cannot be found (HTTP 404).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NotFoundException"/> class.
/// </remarks>
/// <param name="message">The error message. Defaults to "Resource not found".</param>
/// <param name="errors">An optional list of specific error messages.</param>
public class NotFoundException(string message = "Resource not found.", List<string>? errors = null) : AppException(message, StatusCodes.Status404NotFound, errors) { }
