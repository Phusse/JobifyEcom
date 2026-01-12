using Jobify.Api.Models;
using Jobify.Application.Exceptions;

namespace Jobify.Api.Extensions.Responses;

internal static class AppExceptionExtensions
{
    extension(AppException exception)
    {
        public ApiResponse<T> ToApiResponse<T>()
        {
            return new ApiResponse<T>(
                Success: false,
                MessageId: exception.Id,
                Message: exception.Message,
                Details: exception.Details,
                Data: default
            );
        }
    }
}
