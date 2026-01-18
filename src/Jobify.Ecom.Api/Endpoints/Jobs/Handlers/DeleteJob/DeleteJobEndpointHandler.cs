using Jobify.Ecom.Api.Extensions.Claims;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Jobs.DeleteJob;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Jobs.Handlers.DeleteJob;

internal static class DeleteJobEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid id, HttpContext httpContext, IMediator mediator)
    {
        Guid? internalUserId = httpContext.User.GetInternalUserId();

        DeleteJobCommand command = new(id, internalUserId);
        OperationResult<object> result = await mediator.Send(command);

        return Results.Ok(result.ToApiResponse());
    }
}
