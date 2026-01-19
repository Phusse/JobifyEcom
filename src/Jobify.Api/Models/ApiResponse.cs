using Jobify.Application.Models;

namespace Jobify.Api.Models;

internal record ApiResponse<T>(
    bool Success,
    string MessageId,
    string Message,
    List<ResponseDetail>? Details,
    T? Data
);
