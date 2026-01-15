using Jobify.Ecom.Api.Constants.Routes;
using Jobify.Ecom.Api.Endpoints.Base.Handlers;

namespace Jobify.Ecom.Api.Endpoints.Base;

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
