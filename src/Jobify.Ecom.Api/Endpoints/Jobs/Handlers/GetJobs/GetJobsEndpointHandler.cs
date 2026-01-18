using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.GetJobs;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers.GetJobs;

internal static class GetJobsEndpointHandler
{
    public static async Task<IResult> Handle([FromQuery] int pageSize, [FromQuery] DateTime? lastCreatedAt, [FromQuery] Guid? lastJobId, IMediator mediator)
    {
        int effectivePageSize = pageSize <= 0 ? 10 : pageSize;

        GetJobsQuery query = new(
            PageSize: effectivePageSize,
            LastCreatedAt: lastCreatedAt,
            LastJobId: lastJobId
        );

        OperationResult<IReadOnlyList<JobResponse>> result = await mediator.Send(query);

        return Results.Ok(result.ToApiResponse());
    }
}
