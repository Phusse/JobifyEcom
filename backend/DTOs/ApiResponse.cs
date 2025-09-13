namespace JobifyEcom.DTOs;

/// <summary>
/// Standard API response wrapper that provides a consistent structure for all API results.
/// </summary>
/// <typeparam name="T">The type of the data payload returned in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// A unique identifier used to trace the request across services.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Indicates whether the API operation was successful.
    /// </summary>
    public required bool Success { get; set; }

    /// <summary>
    /// A machine-readable code that identifies the response type, allowing clients
    /// to handle cases (e.g., "ACCOUNT_LOCKED") without parsing message text.
    /// </summary>
    public required string MessageId { get; set; }

    /// <summary>
    /// A message describing the outcome of the operation (e.g., success or error message).
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// A list of errors related to the operation, such as validation or warnings.
    /// Can be used for both success and failure responses.
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// The UTC timestamp indicating when the response was generated.
    /// The value is set automatically by the backend.
    /// </summary>
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional data returned by the API. May be null for operations that do not return content.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    public static ApiResponse<T> Ok(T? data = default, string? message = null, List<string>? errors = null, string? traceId = null, string messageId = "UNKNOWN") => new()
    {
        TraceId = traceId,
        MessageId = string.IsNullOrWhiteSpace(messageId) ? "UNKNOWN" : messageId,
        Success = true,
        Message = string.IsNullOrWhiteSpace(message) ? "Operation successful." : message,
        Data = data,
        Errors = errors,
    };

    /// <summary>
    /// Creates a failed API response.
    /// </summary>
    public static ApiResponse<T> Fail(T? data = default, string? message = null, List<string>? errors = null, string? traceId = null, string messageId = "UNKNOWN") => new()
    {
        TraceId = traceId,
        MessageId = string.IsNullOrWhiteSpace(messageId) ? "UNKNOWN" : messageId,
        Success = false,
        Message = string.IsNullOrWhiteSpace(message) ? "Operation failed." : message,
        Data = data,
        Errors = errors,
    };
}
