using Jobify.Api.Services;

namespace Jobify.Api;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApiServices()
        {
            services.AddSingleton<CookieService>();

            return services;
        }
    }
}
