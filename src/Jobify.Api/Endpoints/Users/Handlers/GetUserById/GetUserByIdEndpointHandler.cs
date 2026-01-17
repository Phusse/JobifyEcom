using Jobify.Api.Extensions.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Users.GetUserById;
using Jobify.Application.Features.Users.Models;
using Jobify.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Api.Endpoints.Users.Handlers.GetUserById;

internal static class GetUserByIdEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid id, IMediator mediator)
    {
        OperationResult<UserResponse> result = await mediator.Send(new GetUserByIdQuery(id));

        return Results.Ok(result.ToApiResponse());
    }
}
