using Jobify.Ecom.Api.Services;

namespace Jobify.Ecom.Api;

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
