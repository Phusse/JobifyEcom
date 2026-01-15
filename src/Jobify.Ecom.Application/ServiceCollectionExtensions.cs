using Jobify.Ecom.Application.Configurations.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jobify.Ecom.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices(IConfiguration configuration)
        {
            services.Configure<SessionManagementOptions>(configuration.GetSection("SessionManagement"));
            // services.AddScoped<SessionManagementService>();

            return services;
        }
    }
}
