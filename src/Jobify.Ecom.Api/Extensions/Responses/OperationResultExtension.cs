using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Api.Extensions.Responses;

internal static class OperationResultExtension
{
    extension<T>(OperationResult<T> result)
    {
        public OperationResult<object> WithoutData() => new(
            MessageId: result.MessageId,
            Message: result.Message,
            Details: result.Details,
            Data: null
        );

        public ApiResponse<T> ToApiResponse() => new(
            Success: true,
            MessageId: result.MessageId,
            Message: result.Message,
            Details: result.Details,
            Data: result.Data
        );
    }
}
