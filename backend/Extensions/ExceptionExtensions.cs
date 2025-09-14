using JobifyEcom.DTOs;
using JobifyEcom.Exceptions;

namespace JobifyEcom.Extensions;

/// <summary>
/// Internal extension methods for mapping exceptions to <see cref="ApiResponse{T}"/> where T is object.
/// </summary>
internal static class ExceptionExtensions
{
	/// <summary>
	/// Converts  <see cref="AppException"/> to a failed <see cref="ApiResponse{T}"/> where T is object.
	/// and <see cref="AppException.Details"/> as the errors list.
	/// </summary>
	/// <param name="ex">The <see cref="AppException"/> instance.</param>
	/// <param name="message">Optional developer message (e.g., exception.Message).</param>
	/// <param name="traceId">Optional trace ID.</param>
	/// <returns>A failed <see cref="ApiResponse{T}"/> where T is object; representing the exception.</returns>
	internal static ApiResponse<object> MapToApiResponse(this AppException ex, string? message, string? traceId = null)
	{
		return ApiResponse<object>.Fail(
			data: null,
			message: string.IsNullOrWhiteSpace(message) ? ex.Message : message,
			details: ex.Details,
			traceId: traceId,
			messageId: ex.Id
		);
	}

	/// <summary>
	/// Converts a generic <see cref="Exception"/> to a failed <see cref="ApiResponse{T}"/> where T is  object.
	/// Provides a safe default message and optional developer message.
	/// </summary>
	/// <param name="ex">The exception instance.</param>
	/// <param name="message">Optional developer message (e.g., exception.Message).</param>
	/// <param name="traceId">Optional trace ID.</param>
	/// <returns>A failed <see cref="ApiResponse{T}"/> where T is object representing the exception.</returns>
	internal static ApiResponse<object> MapToApiResponse(this Exception ex, string? message = null, string? traceId = null)
	{
		string errorMessage = string.IsNullOrWhiteSpace(message)
			? "A server error occurred and your request could not be completed. Please try again later or contact support with the trace ID."
			: message;

		return ApiResponse<object>.Fail(
			data: null,
			message: errorMessage,
			details: null,
			traceId: traceId
		);
	}
}
