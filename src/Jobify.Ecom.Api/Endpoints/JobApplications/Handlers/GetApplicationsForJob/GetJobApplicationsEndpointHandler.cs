using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.GetApplicationsForJob;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.GetApplicationsForJob;

internal static class GetApplicationsForJobEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid jobId, [FromQuery] int pageSize, [FromQuery] DateTime? lastCreatedAt, [FromQuery] Guid? lastApplicationId, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();
        int effectivePageSize = pageSize <= 0 ? 10 : pageSize;

        GetApplicationsForJobQuery query = new(
            JobId: jobId,
            RequestingUserId: internalUserId,
            PageSize: effectivePageSize,
            LastCreatedAt: lastCreatedAt,
            LastApplicationId: lastApplicationId
        );

        OperationResult<IReadOnlyList<JobApplicationResponse>> result = await mediator.Send(query);

        return Results.Ok(result.ToApiResponse());
    }
}
