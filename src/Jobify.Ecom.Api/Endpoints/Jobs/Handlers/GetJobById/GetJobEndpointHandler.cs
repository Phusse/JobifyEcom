using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.GetJobById;
using Jobify.Ecom.Application.Features.Jobs.Models;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers.GetJobById;

internal static class GetJobByIdEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid id, IMediator mediator)
    {
        GetJobByIdQuery query = new(id);
        OperationResult<JobResponse> result = await mediator.Send(query);

        return Results.Ok(result.ToApiResponse());
    }
}
