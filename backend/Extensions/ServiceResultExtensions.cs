using JobifyEcom.DTOs;

namespace JobifyEcom.Extensions;

/// <summary>
/// Extension methods for converting <see cref="ServiceResult{T}"/> to <see cref="ApiResponse{T}"/>.
/// </summary>
public static class ServiceResultExtensions
{
	/// <summary>
	/// Maps a <see cref="ServiceResult{T}"/> to an <see cref="ApiResponse{T}"/> for consistent controller responses.
	/// </summary>
	/// <typeparam name="T">The type of data returned.</typeparam>
	/// <param name="result">The service result to convert.</param>
	/// <param name="success">Whether the operation succeeded (defaults to true).</param>
	/// <param name="traceId">Optional trace identifier.</param>
	/// <returns>An <see cref="ApiResponse{T}"/> with the mapped data, message, and errors.</returns>
	public static ApiResponse<T> ToApiResponse<T>(this ServiceResult<T> result, bool success = true, string? traceId = null)
	{
		return success
			? ApiResponse<T>.Ok(result.Data, result.Message, result.Errors, traceId)
			: ApiResponse<T>.Fail(result.Data, result.Message, result.Errors, traceId);
	}
}
