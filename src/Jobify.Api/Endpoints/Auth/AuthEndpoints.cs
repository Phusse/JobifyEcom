using Jobify.Api.Extensions.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.RegisterUser;
using Jobify.Application.Models;

namespace Jobify.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("api/auth");

        authGroup.MapPost("register", async (RegisterUserRequest request, IMediator mediator) =>
        {
            OperationResult<Guid> result = await mediator.Send(request);
            return Results.Ok(result.ToApiResponse());
        })
        .WithName("RegisterUser");
    }
}
