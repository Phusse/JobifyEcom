using Jobify.Ecom.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Ecom.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices(IConfiguration _)
        {
            services.AddScoped<UserIdentityService>();

            return services;
        }
    }
}
