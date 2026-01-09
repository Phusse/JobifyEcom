using Jobify.Api.Models;
using Jobify.Application.Models;

namespace Jobify.Api.Extensions.Responses;

public static class OperationResultExtension
{
    extension<T>(OperationResult<T> result)
    {
        public OperationResult<object> WithoutData()
        {
            return new OperationResult<object>(
                MessageId: result.MessageId,
                Message: result.Message,
                Details: result.Details,
                Data: null
            );
        }

        public ApiResponse<T> ToApiResponse()
        {
            return new ApiResponse<T>(
                Success: true,
                MessageId: result.MessageId,
                Message: result.Message,
                Details: result.Details,
                Data: result.Data
            );
        }
    }
}
