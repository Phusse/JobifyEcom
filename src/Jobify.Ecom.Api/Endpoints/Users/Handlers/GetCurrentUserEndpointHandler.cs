using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Users.GetCurrentUser;
using Jobify.Ecom.Application.Features.Users.Models;
using Jobify.Ecom.Application.Models;
using System.Security.Claims;

namespace Jobify.Ecom.Api.Endpoints.Users.Handlers;

public static class GetCurrentUserEndpointHandler
{
    public static async Task<IResult> Handle(HttpContext context, IMediator mediator)
    {
        string? rawUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = Guid.TryParse(rawUserId, out Guid parsedUserId) ? parsedUserId : null;

        OperationResult<UserResponse> result = await mediator.Send(new GetCurrentUserRequest(userId));

        return Results.Ok(result.ToApiResponse());
    }
}
