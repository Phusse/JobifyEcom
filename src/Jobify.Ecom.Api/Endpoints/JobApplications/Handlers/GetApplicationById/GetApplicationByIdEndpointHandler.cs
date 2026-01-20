using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.GetApplicationById;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.GetApplicationById;

internal static class GetApplicationByIdEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid applicationId, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();

        GetApplicationByIdQuery query = new(
            ApplicationId: applicationId,
            RequestingUserId: internalUserId
        );

        OperationResult<JobApplicationResponse> result = await mediator.Send(query);

        return Results.Ok(result.ToApiResponse());
    }
}
