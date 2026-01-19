using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Api.Models;

internal record ApiResponse<T>(
    bool Success,
    string MessageId,
    string Message,
    List<ResponseDetail>? Details,
    T? Data
);
