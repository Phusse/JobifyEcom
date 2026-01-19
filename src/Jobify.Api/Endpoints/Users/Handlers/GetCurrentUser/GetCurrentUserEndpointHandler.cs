using Jobify.Api.Extensions.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Users.GetCurrentUser;
using Jobify.Application.Features.Users.Models;
using Jobify.Application.Models;
using System.Security.Claims;

namespace Jobify.Api.Endpoints.Users.Handlers.GetCurrentUser;

internal static class GetCurrentUserEndpointHandler
{
    public static async Task<IResult> Handle(HttpContext context, IMediator mediator)
    {
        string? rawUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = Guid.TryParse(rawUserId, out Guid parsedUserId) ? parsedUserId : null;

        OperationResult<UserResponse> result = await mediator.Send(new GetCurrentUserQuery(userId));

        return Results.Ok(result.ToApiResponse());
    }
}
