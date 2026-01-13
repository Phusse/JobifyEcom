using Jobify.Application.Common.Responses;
using Jobify.Application.Exceptions;

namespace Jobify.Application.Extensions.Responses;

internal static class OperationFailureResponseExtensions
{
    extension(OperationFailureResponse response)
    {
        public AppException ToException() => new(response);
    }
}
