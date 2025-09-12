using JobifyEcom.DTOs;
using JobifyEcom.Exceptions;

public static class ApiResponseExtensions
{
	/// <summary>
	/// Maps an <see cref="ErrorDefinition"/> to an <see cref="ApiResponse{T}"/>.
	/// </summary>
	/// <param name="error">The error definition to map.</param>
	/// <param name="data">Optional data payload (default: null).</param>
	/// <param name="traceId">Optional trace ID for the response.</param>
	/// <typeparam name="T">The type of the data payload.</typeparam>
	/// <returns>An <see cref="ApiResponse{T}"/> representing the error.</returns>
	public static ApiResponse<T> ToApiResponse<T>(
		this ErrorDefinition error,
		T? data = default,
		string? traceId = null)
	{
		return ApiResponse<T>.Fail(
			data: data,
			message: error.Title,
			errors: error.Details?.ToList() ?? new List<string>(),
			traceId: traceId
		);
	}
}

public class Testing
{
	public void Testings()
	{
		var ggs = new ErrorDefinition("200 response", 200, "Sucess", ["One"])
			.WithDetails("Two", "Three")
			.ToApiResponse<object>();
	}
}