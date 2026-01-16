using System.Security.Claims;
using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Auth.RegisterUser;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Extensions.Responses;

namespace Jobify.Ecom.Api.Endpoints.Auth.Handlers;

internal static class RegisterUserEndpointHandler
{
    public static async Task<IResult> Handle(HttpContext httpContext, IMediator mediator)
    {
        Guid? sourceUserId = null;
        string? claimValue = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(claimValue, out Guid parsed))
            sourceUserId = parsed;

        RegisterUserRequest request = new(sourceUserId);
        OperationResult<Guid> result = await mediator.Send(request);

        string location = $"{ApiRoutes.User.Base}/{result.Data}";

        ApiResponse<object> response = result.WithoutData().ToApiResponse();

        return Results.Created(location, response);
    }
}
