using JobifyEcom.DTOs;

namespace JobifyEcom.Extensions;

/// <summary>
/// Provides internal extension methods for converting <see cref="ServiceResult{T}"/> to <see cref="ApiResponse{T}"/>.
/// </summary>
internal static class ServiceResultExtensions
{
    extension<T>(ServiceResult<T> result)
    {
        /// <summary>
        /// Maps a <see cref="ServiceResult{T}"/> to an <see cref="ApiResponse{T}"/> for consistent controller responses.
        /// </summary>
        /// <param name="success">Whether the operation succeeded (defaults to true).</param>
        /// <param name="traceId">Optional trace identifier.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> with the mapped data, message, and errors.</returns>
        internal ApiResponse<T> MapToApiResponse(bool success = true, string? traceId = null)
        {
            return success
                ? ApiResponse<T>.Ok(result.Data, result.Message, result.Details, traceId, result.MessageId)
                : ApiResponse<T>.Fail(result.Data, result.Message, result.Details, traceId, result.MessageId);
        }
    }
}
