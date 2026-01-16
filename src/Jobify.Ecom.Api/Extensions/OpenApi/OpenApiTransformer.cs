using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Jobify.Ecom.Api.Extensions.OpenApi;

internal static class OpenApiOptionsExtensions
{
    extension(OpenApiOptions options)
    {
        public void AddCustomOpenApiTransformer()
        {
            options.AddDocumentTransformer((document, context, _) =>
            {
                IServiceProvider services = context.ApplicationServices;
                var env = services.GetRequiredService<IHostEnvironment>();
                var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();

                document.Info.Title = "Jobify Ecom API";
                document.Info.Version = "v1.0.0";

                string readmePath = Path.Combine(env.ContentRootPath, "README.md");

                if (File.Exists(readmePath))
                    document.Info.Description = File.ReadAllText(readmePath);

                HttpContext? httpContext = httpContextAccessor.HttpContext;

                if (httpContext is not null)
                {
                    HttpRequest request = httpContext.Request;
                    string serverUrl = $"{request.Scheme}://{request.Host}";

                    document.Servers =
                    [
                        new OpenApiServer()
                        {
                            Url = serverUrl,
                            Description = env.EnvironmentName,
                        },
                    ];
                }

                return Task.CompletedTask;
            });
        }
    }
}
