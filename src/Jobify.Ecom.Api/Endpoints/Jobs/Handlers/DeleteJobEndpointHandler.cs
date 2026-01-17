using System.Security.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.DeleteJob;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers;

internal static class DeleteJobEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext httpContext,
        IMediator mediator)
    {
        var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return Results.Unauthorized();
        }

        var command = new DeleteJobCommand(id, userId);
        OperationResult<Guid> result = await mediator.Send(command);

        return Results.Ok(result.ToApiResponse());
    }
}
