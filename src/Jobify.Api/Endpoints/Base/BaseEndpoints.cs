using Jobify.Api.Constants.Routes;
using Jobify.Api.Endpoints.Base.Handlers.GetInfo;

namespace Jobify.Api.Endpoints.Base;

internal static class BaseEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapBaseEndpoints()
        {
            app.MapGet(ApiRoutes.ApiBasePath, GetInfoEndpointHandler.Handle)
                .ExcludeFromDescription();
        }
    }
}
