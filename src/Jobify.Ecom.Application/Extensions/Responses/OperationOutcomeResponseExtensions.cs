using Jobify.Ecom.Application.Common.Responses;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Extensions.Responses;

internal static class OperationOutcomeResponseExtensions
{
    private static void NormalizeResponse(IOperationResponse response, out string messageId, out string message, out List<ResponseDetail>? details)
    {
        messageId = string.IsNullOrWhiteSpace(response.Id)
            ? "UNKNOWN_ID"
            : response.Id;

        message = string.IsNullOrWhiteSpace(response.Title)
            ? "No message provided."
            : response.Title;

        details = response.Details is { Length: > 0 }
            ? [.. response.Details]
            : null;
    }

    extension<T>(OperationOutcomeResponse<T> response)
    {
        public OperationResult<T> ToOperationResult()
        {
            NormalizeResponse(response, out string messageId, out string message, out List<ResponseDetail>? details);
            return new OperationResult<T>(messageId, message, details, response.Data);
        }
    }

    extension(OperationOutcomeResponse response)
    {
        public OperationResult<object?> ToOperationResult()
        {
            NormalizeResponse(response, out string messageId, out string message, out List<ResponseDetail>? details);
            return new OperationResult<object?>(messageId, message, details, response.Data);
        }
    }
}
