namespace Jobify.Application.Models;

public record OperationResult<T>(
    string MessageId,
    string Message,
    List<ResponseDetail>? Details,
    T? Data
);
