using Jobify.Api.Constants.Routes;
using Jobify.Api.Extensions.Responses;
using Jobify.Api.Models;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.RegisterUser;
using Jobify.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Api.Endpoints.Auth.Handlers;

internal static class RegisterUserEndpointHandler
{
    public static async Task<IResult> Handle([FromBody] RegisterUserRequest request, IMediator mediator)
    {
        OperationResult<Guid> result = await mediator.Send(request);

        Guid userId = result.Data;
        string location = $"{ApiRoutes.User.Base}/{userId}";

        ApiResponse<object> response = result.WithoutData()
            .ToApiResponse();

        return Results.Created(location, response);
    }
}
