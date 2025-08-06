namespace JobifyEcom.Exceptions;

/// <summary>
/// Represents an exception for unauthorized access (HTTP 401).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
/// </remarks>
/// <param name="message">The error message. Defaults to "Unauthorized access".</param>
/// <param name="errors">An optional list of specific error messages.</param>
public class UnauthorizedException(string message = "Unauthorized access.", List<string>? errors = null) : AppException(message, StatusCodes.Status401Unauthorized, errors) { }
