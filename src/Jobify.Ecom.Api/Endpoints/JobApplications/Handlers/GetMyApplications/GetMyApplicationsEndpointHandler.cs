using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.JobApplications.GetMyApplications;
using Jobify.Ecom.Application.Features.JobApplications.Models;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.JobApplications.Handlers.GetMyApplications;

internal static class GetMyApplicationsEndpointHandler
{
    public static async Task<IResult> Handle([FromQuery] int pageSize, [FromQuery] DateTime? lastCreatedAt, [FromQuery] Guid? lastApplicationId, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();
        int effectivePageSize = pageSize <= 0 ? 10 : pageSize;

        GetMyApplicationsQuery query = new(
            ApplicantUserId: internalUserId,
            PageSize: effectivePageSize,
            LastCreatedAt: lastCreatedAt,
            LastApplicationId: lastApplicationId
        );

        OperationResult<IReadOnlyList<JobApplicationResponse>> result = await mediator.Send(query);

        return Results.Ok(result.ToApiResponse());
    }
}
