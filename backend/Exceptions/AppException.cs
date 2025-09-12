namespace JobifyEcom.Exceptions;

/// <summary>
/// The base class for all custom application exceptions.
/// Encapsulates an HTTP status code, a unique code, and optionally a list of error messages.
/// </summary>
public class AppException : Exception
{
	/// <summary>
	/// Gets the HTTP status code associated with the exception.
	/// </summary>
	public int StatusCode { get; }

	/// <summary>
	/// Gets the unique identifier for the exception (optional).
	/// </summary>
	public string? Code { get; }

	/// <summary>
	/// Gets the list of detailed error messages, if provided.
	/// </summary>
	public List<string>? Errors { get; }

	/// <summary>
	/// Initializes a new instance of <see cref="AppException"/>.
	/// </summary>
	/// <param name="statusCode">The HTTP status code to return.</param>
	/// <param name="message">The main error message.</param>
	/// <param name="errors">
	/// An optional list of specific error messages.
	/// If null or empty, <see cref="Errors"/> will remain null.
	/// </param>
	/// <param name="code">An optional unique error code.</param>
	[Obsolete("Depreacted, Use the other constructor that accepts error definition.")]
	public AppException(int statusCode, string message, List<string>? errors = null, string? code = null) : base(message)
	{
		StatusCode = statusCode;
		Code = code;
		Errors = (errors is not null && errors.Count > 0) ? errors : null;
	}

	/// <summary>
	/// Initializes a new instance of <see cref="AppException"/> using an <see cref="ErrorDefinition"/>.
	/// </summary>
	/// <param name="error">The predefined error definition.</param>
	public AppException(ErrorDefinition error) : base(error.Title)
	{
		StatusCode = error.HttpStatus;
		Code = error.Code;
		Errors = (error.Details.Length > 0) ? [.. error.Details] : null;
	}
}
