using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Endpoints.Auth.Handlers;
using Jobify.Ecom.Api.Models;
using Scalar.AspNetCore;

namespace Jobify.Ecom.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapAuthEndpoints()
        {
            RouteGroupBuilder authGroup = app.MapGroup(ApiRoutes.Auth.Base)
                .WithTags("Authentication");

            authGroup.MapPost(ApiRoutes.Auth.Register, RegisterUserEndpointHandler.Handle)
                .WithName(nameof(RegisterUserEndpointHandler))
                .WithSummary("Register a new user")
                .WithDescription(
                    """
                    Registers a new user on the Jobify Ecom platform using your existing authentication credentials
                    from the API Gateway.

                    Requirements:
                    - You must be already authenticated through the API Gateway.
                    - Your data (e.g., `UserId`) from the gateway will be used to register you in the Ecom platform.

                    On success, the `Location` header contains the URI of the newly registered user.

                    Possible responses:
                    - `201 Created`: User successfully registered.
                    - `409 Conflict`: Email already in use.
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();
        }
    }
}
