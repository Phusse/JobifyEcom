namespace JobifyEcom.DTOs;

/// <summary>
/// Represents the result of a service operation, including data, a message, and optional warnings or errors.
/// Exceptions should be thrown for failures instead of using a success flag.
/// </summary>
/// <typeparam name="T">The type of the data returned by the service.</typeparam>
public class ServiceResult<T>
{
	/// <summary>
	/// An informational message about the operation.
	/// </summary>
	public string? Message { get; set; }

	/// <summary>
	/// A list of warnings or non-fatal errors related to the operation.
	/// </summary>
	public List<string>? Errors { get; set; }

	/// <summary>
	/// The data returned by the service. May be null.
	/// </summary>
	public T? Data { get; set; }

	/// <summary>
	/// Creates a new <see cref="ServiceResult{T}"/> instance.
	/// </summary>
	/// <param name="data">The data returned by the service.</param>
	/// <param name="message">An optional informational message.</param>
	/// <param name="errors">Optional list of warnings or errors.</param>
	/// <returns>A new <see cref="ServiceResult{T}"/> with the specified data and message.</returns>
	public static ServiceResult<T> Create(T? data, string? message = null, List<string>? errors = null) => new()
	{
		Data = data,
		Message = message,
		Errors = errors,
	};
}
