using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.GetJobs;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers;

internal static class GetJobsEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        IMediator mediator)
    {
        var query = new GetJobsRequest(page == 0 ? 1 : page, pageSize == 0 ? 10 : pageSize);
        OperationResult<IEnumerable<Job>> result = await mediator.Send(query);

        return Results.Ok(result.ToApiResponse());
    }
}
