using Jobify.Api.Constants.Cookies;
using Jobify.Api.Constants.Routes;
using Jobify.Api.Endpoints.Auth.Handlers.LoginUser;
using Jobify.Api.Endpoints.Auth.Handlers.LogoutUser;
using Jobify.Api.Endpoints.Auth.Handlers.RefreshSession;
using Jobify.Api.Endpoints.Auth.Handlers.RegisterUser;
using Jobify.Api.Models;
using Jobify.Application.Features.Auth.Models;
using Scalar.AspNetCore;

namespace Jobify.Api.Endpoints.Auth;

internal static class AuthEndpoints
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
                    Registers a new user with the provided details:

                    - `UserName`: Desired username (trimmed).
                    - `Email`: User's email (hashed internally).
                    - `Password`: User's password (hashed internally).
                    - `FirstName`, `MiddleName`, `LastName`: Personal names (encrypted).

                    On success, the `Location` header contains the URI of the new user.

                    Possible responses:
                    - `201 Created`: User successfully registered.
                    - `400 BadRequest`: Invalid or missing input.
                    - `409 Conflict`: Email already in use.
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            authGroup.MapPost(ApiRoutes.Auth.Login, LoginUserEndpointHandler.Handle)
                .WithName(nameof(LoginUserEndpointHandler))
                .WithSummary("Login a user")
                .WithDescription(
                    $"""
                    Logs in a user and creates a new session:

                    - `Identifier`: Email or username.
                    - `Password`: User's password.
                    - `RememberMe`: Optional; if true, session lasts 7 days, otherwise 8 hours.

                    On success, a secure HTTP-only cookie `{CookieKeys.Session}` is set to manage the session.

                    Possible responses:
                    - `200 OK`: Login successful.
                    - `400 BadRequest`: Invalid input.
                    - `401 Unauthorized`: Incorrect credentials or account locked.
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<SessionTimestampsResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            authGroup.MapPost(ApiRoutes.Auth.Refresh, RefreshSessionEndpointHandler.Handle)
                .WithName(nameof(RefreshSessionEndpointHandler))
                .WithSummary("Refresh user session")
                .WithDescription(
                    $"""
                    Refreshes the current user session, extending its expiration
                    **only if the session is within the allowed refresh threshold**.

                    **Authentication Required**: A valid session is needed.

                    On success, a new secure HTTP-only `{CookieKeys.Session}` cookie is set with updated timestamps.

                    Possible responses:
                    - `200 OK`: Session successfully refreshed.
                    - `401 Unauthorized`: No valid session, session expired, or not eligible for refresh yet.
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<SessionTimestampsResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            authGroup.MapPost(ApiRoutes.Auth.Logout, LogoutUserEndpointHandler.Handle)
                .WithName(nameof(LogoutUserEndpointHandler))
                .WithSummary("Logout user")
                .WithDescription(
                    $"""
                    Logs out the current user by revoking their session.

                    **Authentication Required**: A valid session is needed.

                    This endpoint performs the following actions:
                    - Revokes the current session in the database.
                    - Deletes the `{CookieKeys.Session}` cookie from the client.

                    Possible responses:
                    - `200 OK`: Successfully logged out.
                    - `401 Unauthorized`: No valid session (user may already be logged out).
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();
        }
    }
}
