namespace Jobify.Ecom.Application.Models;

public record OperationResult<T>(
    string MessageId,
    string Message,
    List<ResponseDetail>? Details,
    T? Data
);
