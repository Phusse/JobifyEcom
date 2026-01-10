using Jobify.Api.Constants.Routes;
using Jobify.Api.Endpoints.Auth.Handlers;
using Jobify.Api.Models;
using Scalar.AspNetCore;

namespace Jobify.Api.Endpoints.Auth;

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
                    Registers a new user account with the provided details:

                    - `UserName`: The desired username (trimmed of whitespace).
                    - `Email`: The user's email address (hashed internally).
                    - `Password`: The user's password (hashed internally).
                    - `FirstName`, `MiddleName`, `LastName`: Personal names stored encrypted.

                    On success, the `Location` header contains the URI of the newly created user.

                    Possible responses:
                    - `201 Created`: User successfully registered.
                    - `400 BadRequest`: Invalid input data (e.g., missing required fields).
                    - `409 Conflict`: A user with the same email already exists.
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);
        }
    }
}
