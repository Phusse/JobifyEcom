using Jobify.Api.Constants.Routes;
using Jobify.Api.Endpoints.Base.Handlers;

namespace Jobify.Api.Endpoints.Base;

public static class BaseEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapBaseEndpoints()
        {
            app.MapGet(ApiRoutes.BasePath, GetInfoEndpointHandler.Handle)
                .ExcludeFromDescription();
        }
    }
}
