using Jobify.Api.Constants.Routes;
using Jobify.Api.Endpoints.Users.Handlers;
using Jobify.Api.Models;
using Jobify.Application.Features.Users.Models;

namespace Jobify.Api.Endpoints.Users;

public static class UserEndpoints
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
                .Produces<ApiResponse<UserResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            userGroup.MapGet(ApiRoutes.User.GetById, GetUserByIdEndpointHandler.Handle)
                .WithName(nameof(GetUserByIdEndpointHandler))
                .WithSummary("Get user by ID")
                .Produces<ApiResponse<UserResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
        }
    }
}
