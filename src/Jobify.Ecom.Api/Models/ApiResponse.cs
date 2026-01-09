using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Api.Models;

public record ApiResponse<T>(
    bool Success,
    string MessageId,
    string Message,
    List<ResponseDetail>? Details,
    T? Data
)
{
    public DateTime Timestamp { get; private init; } = DateTime.UtcNow;
}
