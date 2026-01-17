using Jobify.Api.Constants.Cookies;
using Jobify.Api.Constants.Routes;
using Jobify.Api.Endpoints.Users.Handlers.GetCurrentUser;
using Jobify.Api.Endpoints.Users.Handlers.GetUserById;
using Jobify.Api.Models;
using Jobify.Application.Features.Users.Models;

namespace Jobify.Api.Endpoints.Users;

internal static class UserEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapUserEndpoints()
        {
            RouteGroupBuilder userGroup = app.MapGroup(ApiRoutes.User.Base)
                .WithTags("Users");

            userGroup.MapGet(ApiRoutes.User.Me, GetCurrentUserEndpointHandler.Handle)
                .WithName(nameof(GetCurrentUserEndpointHandler))
                .WithSummary("Get current authenticated user")
                .WithDescription(
                    $"""
                    Retrieves the profile of the currently authenticated user.

                    **Authentication Required**: A valid session (`{CookieKeys.Session}` cookie) is needed.

                    The response includes:
                    - `Id`: User's unique identifier.
                    - `UserName`: Display username.
                    - `FirstName`, `MiddleName`, `LastName`: Personal names.
                    - `Role`: User's system role (e.g., Admin, User).

                    Possible responses:
                    - `200 OK`: User profile successfully retrieved.
                    - `401 Unauthorized`: No valid session or session expired.
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<UserResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();

            userGroup.MapGet(ApiRoutes.User.GetById, GetUserByIdEndpointHandler.Handle)
                .WithName(nameof(GetUserByIdEndpointHandler))
                .WithSummary("Get user by ID")
                .WithDescription(
                    $"""
                    Retrieves the public profile of a user by their unique ID.

                    **Authentication Required**: A valid session (`{CookieKeys.Session}` cookie) is needed.

                    **Route Parameters**:
                    - `id` (GUID): The unique identifier of the user to retrieve.

                    The response includes:
                    - `Id`: User's unique identifier.
                    - `UserName`: Display username.
                    - `FirstName`, `MiddleName`, `LastName`: Personal names.
                    - `Role`: User's system role.

                    Possible responses:
                    - `200 OK`: User profile successfully retrieved.
                    - `401 Unauthorized`: No valid session or session expired.
                    - `404 NotFound`: No user exists with the specified ID.
                    - `500 InternalServerError`: Unexpected server error.
                    """
                )
                .Produces<ApiResponse<UserResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization();
        }
    }
}
