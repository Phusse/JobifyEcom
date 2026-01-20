using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.UpdateApplicationStatus;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.UpdateApplicationStatus;

internal static class UpdateApplicationStatusEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid applicationId, [FromBody] UpdateApplicationStatusRequest request, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();

        UpdateApplicationStatusCommand command = new(
            ApplicationId: applicationId,
            NewStatus: request.NewStatus,
            RequestingUserId: internalUserId
        );

        OperationResult<object> result = await mediator.Send(command);

        return Results.Ok(result.ToApiResponse());
    }
}
