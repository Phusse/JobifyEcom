using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Auth.RegisterUser;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Models;
using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Api.Constants.Auth;
using System.Security.Claims;

namespace Jobify.Ecom.Api.Endpoints.Auth.Handlers.RegisterUser;

internal static class RegisterUserEndpointHandler
{
    public static async Task<IResult> Handle(HttpContext httpContext, IMediator mediator)
    {
        Guid? sourceUserId = null;
        Claim? externalClaim = httpContext.User.FindFirst(SessionClaimTypes.ExternalUserId);

        if (externalClaim is not null && Guid.TryParse(externalClaim.Value, out Guid parsed))
            sourceUserId = parsed;

        RegisterUserCommand request = new(sourceUserId);
        OperationResult<Guid> result = await mediator.Send(request);

        string location = $"{ApiRoutes.User.Base}/{result.Data}";

        ApiResponse<object> response = result.WithoutData().ToApiResponse();

        return Results.Created(location, response);
    }
}
