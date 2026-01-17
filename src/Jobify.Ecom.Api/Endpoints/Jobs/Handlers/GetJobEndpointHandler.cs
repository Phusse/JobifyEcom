using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.GetJob;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Domain.Entities.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers;

internal static class GetJobEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid id,
        IMediator mediator)
    {
        var query = new GetJobRequest(id);
        OperationResult<Job?> result = await mediator.Send(query);

        return Results.Ok(result.ToApiResponse());
    }
}
