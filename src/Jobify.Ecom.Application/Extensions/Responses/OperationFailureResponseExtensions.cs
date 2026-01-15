using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Exceptions;

namespace Jobify.Ecom.Application.Extensions.Responses;

internal static class OperationFailureResponseExtensions
{
    extension(OperationFailureResponse response)
    {
        public AppException ToException() => new(response);
    }
}
