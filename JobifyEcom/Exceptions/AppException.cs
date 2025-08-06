namespace JobifyEcom.Exceptions;

/// <summary>
/// The base class for all custom application exceptions.
/// Encapsulates an HTTP status code and a list of error messages.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AppException"/> class.
/// </remarks>
/// <param name="message">The main error message.</param>
/// <param name="statusCode">The HTTP status code to return.</param>
/// <param name="errors"> An optional list of specific error messages.
/// If null or empty, defaults to a single-item list containing, <paramref name="message"/></param>
public class AppException(string message, int statusCode, List<string>? errors = null) : Exception(message)
{
	/// <summary>
	/// Gets the HTTP status code associated with the exception.
	/// </summary>
	public int StatusCode { get; } = statusCode;

	/// <summary>
	/// Gets the list of detailed error messages.
	/// Always contains at least one message.
	/// </summary>
	public List<string> Errors { get; } = string.IsNullOrWhiteSpace(message) && (errors == null || errors.Count == 0)
		? ["An unexpected error occurred."]
		: (errors is { Count: > 0 } ? errors : [message]);
}
