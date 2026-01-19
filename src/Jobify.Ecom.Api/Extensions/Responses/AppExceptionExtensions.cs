using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Exceptions;

namespace Jobify.Ecom.Api.Extensions.Responses;

internal static class AppExceptionExtensions
{
    extension(AppException exception)
    {
        public ApiResponse<T> ToApiResponse<T>() => new(
            Success: false,
            MessageId: exception.Id,
            Message: exception.Message,
            Details: exception.Details,
            Data: default
        );
    }
}
