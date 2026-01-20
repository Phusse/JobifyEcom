using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.WithdrawApplication;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.WithdrawApplication;

internal static class WithdrawApplicationEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid applicationId, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();

        WithdrawApplicationCommand command = new(
            ApplicationId: applicationId,
            ApplicantUserId: internalUserId
        );

        OperationResult<object> result = await mediator.Send(command);

        return Results.Ok(result.ToApiResponse());
    }
}
