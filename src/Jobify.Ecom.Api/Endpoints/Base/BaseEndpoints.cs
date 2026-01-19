using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Endpoints.Base.Handlers.GetInfo;

namespace Jobify.Ecom.Api.Endpoints.Base;

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
