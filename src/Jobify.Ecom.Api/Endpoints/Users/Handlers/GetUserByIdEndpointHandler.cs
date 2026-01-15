using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Users.GetUserById;
using Jobify.Ecom.Application.Features.Users.Models;
using Jobify.Ecom.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Ecom.Api.Endpoints.Users.Handlers;

public static class GetUserByIdEndpointHandler
{
    public static async Task<IResult> Handle([FromRoute] Guid id, IMediator mediator)
    {
        OperationResult<UserResponse> result = await mediator.Send(new GetUserByIdRequest(id));

        return Results.Ok(result.ToApiResponse());
    }
}
